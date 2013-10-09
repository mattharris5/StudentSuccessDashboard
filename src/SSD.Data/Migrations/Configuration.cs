using SSD.Data;
using SSD.Domain;
using SSD.Properties;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace SSD.Migrations
{
    internal sealed class EducationConfiguration : DbMigrationsConfiguration<EducationDataContext>
    {
        public EducationConfiguration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(EducationDataContext context)
        {
            InitializeSystemUser(context);
            InitializeEula(context);
            InitializeSubjects(context);
            InitializePriorities(context);
            InitializeRoles(context);
            InitializeCategories(context);
            InitializeFulfillmentStatus(context);
            InitializeCustomFieldCategory(context);
            InitializeCustomFieldType(context);
            context.SaveChanges();
            InitializeProperties(context);
            ScriptResourceExecutor.ExecuteScript(context.Database, "SSD.EducationDataContext-PostDeploy.sql");
        }

        private static void InitializeSystemUser(EducationDataContext context)
        {
            if (!context.Users.Any())
            {
                context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [SSD].[User] ON; INSERT INTO [SSD].[User] (UserId, Active, UserKey, DisplayName, FirstName, LastName, EmailAddress, ConfirmationGuid, CreateTime) VALUES (-1, 1, '[SYSTEM]', '[SYSTEM]', '[SYSTEM]', '[SYSTEM]', 'Administrator@studentsuccessdashboard.com', cast(cast(0 as binary) as uniqueidentifier), GETDATE()); SET IDENTITY_INSERT [SSD].[User] OFF");
            }
        }

        private static void InitializeEula(EducationDataContext context)
        {
            if (!context.EulaAgreements.Any())
            {
                context.EulaAgreements.Add(new EulaAgreement
                {
                    CreateTime = DateTime.Now,
                    CreatingUserId = -1,
                    EulaText = Resources.DefaultEula
                });
            }
        }

        private static void InitializeSubjects(EducationDataContext context)
        {
            context.Subjects.AddOrUpdate(s => s.Name,
                new Subject { Name = "None" },
                new Subject { Name = "Math" },
                new Subject { Name = "Reading" },
                new Subject { Name = "Science" },
                new Subject { Name = "Social Studies" }
            );
        }

        private static void InitializePriorities(EducationDataContext context)
        {
            context.Priorities.AddOrUpdate(p => p.Name,
                new Priority { Name = "None" },
                new Priority { Name = "Low" },
                new Priority { Name = "Medium" },
                new Priority { Name = "High" }
            );
        }

        private static void InitializeRoles(EducationDataContext context)
        {
            context.Roles.AddOrUpdate(r => r.Name,
                new Role { Name = "Data Admin" },
                new Role { Name = "Site Coordinator" },
                new Role { Name = "Provider" }
            );
        }

        private static void InitializeCategories(EducationDataContext context)
        {
            context.Categories.AddOrUpdate(c => c.Name,
                new Category { Name = "Basic Needs" },
                new Category { Name = "Consumer Services" },
                new Category { Name = "Criminal Justice and Legal Services" },
                new Category { Name = "Education" },
                new Category { Name = "Environmental Quality" },
                new Category { Name = "Health Care" },
                new Category { Name = "Income Support and Employment" },
                new Category { Name = "Individual and Family Life" },
                new Category { Name = "Mental Health Care and Counseling" },
                new Category { Name = "Organizational/Community Services" },
                new Category { Name = "Support Groups" },
                new Category { Name = "Target Populations" }
            );
        }

        private static void InitializeFulfillmentStatus(EducationDataContext context)
        {
            context.FulfillmentStatuses.AddOrUpdate(c => c.Name,
                new FulfillmentStatus { Name = Statuses.Open },
                new FulfillmentStatus { Name = Statuses.Fulfilled },
                new FulfillmentStatus { Name = Statuses.Rejected }
            );
        }

        private static void InitializeCustomFieldCategory(EducationDataContext context)
        {
            context.CustomFieldCategories.AddOrUpdate(c => c.Name,
                new CustomFieldCategory { Name = "Attendance" },
                new CustomFieldCategory { Name = "Behavior" },
                new CustomFieldCategory { Name = "Grades" },
                new CustomFieldCategory { Name = "Student Details" },
                new CustomFieldCategory { Name = "Tests" }
            );
        }

        private static void InitializeCustomFieldType(EducationDataContext context)
        {
            context.CustomFieldTypes.AddOrUpdate(c => c.Name,
                new CustomFieldType { Name = "Integer" },
                new CustomFieldType { Name = "Text" },
                new CustomFieldType { Name = "Rich Text" },
                new CustomFieldType { Name = "Date" },
                new CustomFieldType { Name = "XML" },
                new CustomFieldType { Name = "HTML" },
                new CustomFieldType { Name = "Decimal" }
            );
        }

        private static void InitializeProperties(EducationDataContext context)
        {
            var domainTypes = typeof(Student).Assembly.GetTypes();
            var properties = domainTypes.SelectMany(t => CreatePropertiesFromType(t));
            context.Properties.AddOrUpdate(p => new { p.EntityName, p.Name }, properties.ToArray());
        }

        private static IEnumerable<Property> CreatePropertiesFromType(Type t)
        {
            string[] nonDirectoryLevelProperties = 
            {
                GetMemberName(() => (new Student()).StudentKey),
                GetMemberName(() => (new Student()).ServiceRequests),
                GetMemberName(() => (new Student()).StudentAssignedOfferings),
            };
            var clrProperties = t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            return clrProperties.Select(p => new Property
            {
                EntityName = t.FullName,
                Name = p.Name,
                IsProtected = nonDirectoryLevelProperties.Contains(p.Name)
            });
        }

        private static string GetMemberName<T>(Expression<Func<T>> expr)
        {
            return ((MemberExpression)expr.Body).Member.Name;
        }
    }
}
