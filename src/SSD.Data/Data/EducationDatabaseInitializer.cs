using SSD.Domain;
using SSD.Migrations;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace SSD.Data
{
    internal class EducationDatabaseInitializer : IDatabaseInitializer<EducationDataContext>
    {
        public void InitializeDatabase(EducationDataContext context)
        {
            bool databasePreexistsInitialize = context.Database.Exists();
            MigrateToLatestVersion(context);
            using (TransactionScope transaction = new TransactionScope())
            {
                RunScripts(context.Database, databasePreexistsInitialize);
                SetupInitialState(context, databasePreexistsInitialize);
                transaction.Complete();
            }
        }

        private void MigrateToLatestVersion(EducationDataContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (context.Database.Exists())
            {
                ScriptResourceExecutor.ExecuteScript(context.Database, "SSD.EducationDataContext-PreDeploy.sql");
            }
            EducationConfiguration configuration = CreateMigrationsConfiguration(context);
            DbMigrator migrator = new DbMigrator(configuration);
            migrator.Update();
        }

        private static EducationConfiguration CreateMigrationsConfiguration(EducationDataContext context)
        {
            string connectionString = NormalizeMigrationConnectionString(context);
            EducationConfiguration configuration = new EducationConfiguration();
            configuration.TargetDatabase = new DbConnectionInfo(connectionString, "System.Data.SqlClient");
            return configuration;
        }

        private static string NormalizeMigrationConnectionString(EducationDataContext context)
        {
            string connectionString = EducationDataContext.NormalizeConnectionString(context.Database.Connection.ConnectionString);
            var builder = new SqlConnectionStringBuilder(connectionString);
            if (builder.ApplicationName == null || !builder.ApplicationName.EndsWith(" - Migrations"))
            {
                builder.ApplicationName = (builder.ApplicationName ?? "EF") + " - Migrations";
            }
            return builder.ConnectionString;
        }

        private void RunScripts(Database database, bool databasePreexistsInitialize)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            ScriptResourceExecutor.ExecuteScript(database, "SSD.EducationDataContext.sql");
        }

        private void SetupInitialState(EducationDataContext context, bool databasePreexistsInitialize)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (!databasePreexistsInitialize)
            {
                InitializeSubjects(context);
                InitializePriorities(context);
                InitializeCategories(context);
                InitializeTeachers(context);
                InitializeClasses(context);
                InitializeProviders(context);
                InitializeServiceTypes(context);
                InitializePrograms(context);
                InitializeSchools(context);
                InitializeStudents(context);
                InitializeUsers(context);
                InitializeStudentAssignedOfferings(context);
                InitializeServiceRequests(context);
                InitializeCustomField(context);
                InitializeCustomDataOrigin(context);
                InitializeCustomFieldValues(context);
                InitializeServiceAttendances(context);
                context.SaveChanges();
            }
        }

        [Conditional("DEBUG")]
        private static void InitializeTeachers(EducationDataContext context)
        {
            context.Teachers.Add(new Teacher
            {
                TeacherKey = "506bc8b90e130a4b4b919cf7",
                Email = "tachoge@cpsboe.k12.oh.us",
                FirstName = "George",
                MiddleName = "Frank",
                LastName = "Doddy-Tacho",
                Phone = "(513) 555-1212",
                Number = "1016",
            });
            context.Teachers.Add(new Teacher
            {
                TeacherKey = "606hd8b90e870a4b4b919qh4",
                Email = "tommy@blah.k12.oh.us",
                FirstName = "Tommy",
                MiddleName = "Eugene",
                LastName = "Smith",
                Phone = "(513) 555-9191",
                Number = "1216",
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeClasses(EducationDataContext context)
        {
            context.Classes.Add(new Class { ClassKey = "506bc8630e130a4b4b919110", Name = "LearnSprout Computer Lab 355", Number = "90007", Teacher = context.Teachers.Local[0] });
            context.Classes.Add(new Class { ClassKey = "506bc8630e130a4b4b919115", Name = "LearnSprout Computer Lab 357", Number = "90009", Teacher = context.Teachers.Local[1] });
        }

        [Conditional("DEBUG")]
        private static void InitializeProviders(EducationDataContext context)
        {
            context.Providers.Add(new Provider
            {
                Name = "YMCA",
                Address = new Address { Street = "123 Road St.", City = "Columbus", Zip = "43220", State = "OH" },
                Contact = new Contact { Name = "Bob Smith", Email = "bob@ymca.com", Phone = "614-444-4444" },
                IsActive = true
            });
            context.Providers.Add(new Provider
            {
                Name = "Big Brothers, Big Sisters",
                Address = new Address { Street = "123 Street Rd", City = "Beverly Hills", Zip = "90210", State = "CA" },
                Contact = new Contact { Name = "Emily Smith", Email = "emily@bbbs.com", Phone = "123-444-4444" },
                IsActive = true
            });
            context.Providers.Add(new Provider
            {
                Name = "Boys and Girls Club",
                Address = new Address { Street = "Main Street", City = "Columbus", Zip = "43220", State = "OH" },
                Contact = new Contact { Name = "Yosimite Sam", Email = "ysam@club.com", Phone = "614-777-7777" },
                IsActive = true
            });
            context.Providers.Add(new Provider
            {
                Name = "Joe's World-class Tutoring Services and Eatery!",
                Address = new Address { Street = "83397 County Road 134", City = "Charlotte", Zip = "23521", State = "NC" },
                Contact = new Contact { Name = "Joe", Email = "joe@tutoringandeatery.com", Phone = "457-333-3333" },
                IsActive = true
            });
        }

        [Conditional("DEBUG")]
        private static void InitializePrograms(EducationDataContext context)
        {
            context.Programs.Add(new Program
            {
                StartDate = DateTime.Now,
                Name = "After School Basketball",
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[0],
                        ServiceType = context.ServiceTypes.Local[1],
                        IsActive = true
                    }
                },
                Purpose = "To provide kids with a place to play some basketball",
                ContactInfo = new Contact { Name = "George Washington", Email = "gdubs1776@americaonline.com", Phone = "123-567-8910" },
                IsActive = true
            });
            context.Programs.Add(new Program 
            {
                StartDate = DateTime.Now.AddDays(1),
                Name = "One on One Activities",
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[1],
                        ServiceType = context.ServiceTypes.Local[0],
                        IsActive = true
                    },
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[2],
                        ServiceType = context.ServiceTypes.Local[0],
                        IsActive = true
                    }
                },
                Purpose = "To provide kids with some one on one time",
                IsActive = true
            });
            context.Programs.Add(new Program
            {
                StartDate = DateTime.Now.AddDays(2),
                Name = "Joe's Tutoring!",
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[2],
                        ServiceType = context.ServiceTypes.Local[0],
                        IsActive = true
                    },
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[2],
                        ServiceType = context.ServiceTypes.Local[1],
                        IsActive = true
                    }
                },
                Purpose = "To give kids some awesome tutoring from Joe!",
                IsActive = true
            });
            context.Programs.Add(new Program
            {
                StartDate = DateTime.Now.AddDays(3),
                Name = "More Programs!",
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[0],
                        ServiceType = context.ServiceTypes.Local[0],
                        IsActive = true
                    },
                },
                Purpose = "To give kids some more wicked awesome programs!",
                IsActive = true
            });
            context.Programs.Add(new Program
            {
                StartDate = DateTime.Now.AddDays(3),
                Name = "Even More Programs!",
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        Provider = context.Providers.Local[0],
                        ServiceType = context.ServiceTypes.Local[0],
                        IsActive = true
                    }
                },
                Purpose = "To give kids even more wicked awesome programs!",
                IsActive = true
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeSchools(EducationDataContext context)
        {
            School added = context.Schools.Add(new School { Name = "Local High School", Programs = new List<Program>() });
            added.Programs.Add(context.Programs.Local[0]);
            added.Programs.Add(context.Programs.Local[1]);
            added = context.Schools.Add(new School { Name = "Community Middle School", Programs = new List<Program>() });
            added.Programs.Add(context.Programs.Local[1]);
            added.Programs.Add(context.Programs.Local[2]);
            added = context.Schools.Add(new School { Name = "Springfield Elementary School", Programs = new List<Program>() });
            added.Programs.Add(context.Programs.Local[0]);
            added.Programs.Add(context.Programs.Local[1]);
            added.Programs.Add(context.Programs.Local[2]);
            context.Schools.Add(new School { Name = "My, test, school,", Programs = new List<Program>() });
        }

        [Conditional("DEBUG")]
        private static void InitializeStudents(EducationDataContext context)
        {
            string[] lastNames = new string[] { "Ovadia", "Jain", "Gleckler", "Martin", "Gear", "Zimmerman", "Smith", "Jefferson", "Nganga", "Li", "Perez" };
            string[] firstNames = new string[] { "Micah", "Nidhi", "Mark", "Nick", "Jon", "Geoff", "Charles", "Tom", "Alice", "Shelly", "Dirk" };
            string[] middleNames = new string[] { "Zev", "Quizzno", "Azure", "Columbus", "Cincinnati", "Microsoft", "Ann", "Patricia", "Jones", "Albert", "Einstein" };
            string[] parents = new string[] { "Meir, Sheila", "Apu", "Marilyn", "Sam, Jenny", "Fred, Sally", "The Jetsons", "Jennifer and Eugene" };
            Random randomGenerator = new Random(150);
            for (int i = 0; i < 87; i++)
            {
                Student s = new Student 
                {
                    StudentKey = (i + 1 + 3).ToString(),
                    StudentSISId = (i + 1 * 10).ToString(),
                    LastName = lastNames[randomGenerator.Next(lastNames.Length - 1)],
                    FirstName = firstNames[randomGenerator.Next(firstNames.Length - 1)],
                    MiddleName = middleNames[randomGenerator.Next(middleNames.Length - 1)],
                    Parents = parents[randomGenerator.Next(parents.Length - 1)],
                    Grade = randomGenerator.Next(1, 13),
                    School = context.Schools.Local[randomGenerator.Next(context.Schools.Local.Count - 1)],
                    HasParentalOptOut = i % 3 == 0 && i % 4 == 2
                };
                s.DateOfBirth = new DateTime(DateTime.Today.Year - s.Grade - 5, randomGenerator.Next(1, 12), randomGenerator.Next(1, 28));
                context.Students.Add(s);
                for (int j = 0; j < 2; j++)
                {
                    context.Students.Local[i].Classes.Add(context.Classes.Local[j]);
                    context.Classes.Local[j].Students.Add(context.Students.Local[i]);
                }
                if (i % 3 == 0)
                {
                    if (i % 4 == 0)
                    {
                        s.ApprovedProviders.Add(context.Providers.Local[0]);
                    }
                    if (i % 5 == 0 || i % 7 == 0)
                    {
                        s.ApprovedProviders.Add(context.Providers.Local[1]);
                    }
                    if (i % 2 == 0)
                    {
                        s.ApprovedProviders.Add(context.Providers.Local[2]);
                    }
                }
                if (i % 7 == 2 || i % 7 == 5 && i % 5 == 0)
                {
                    s.ApprovedProviders.Add(context.Providers.Local[3]);
                }
            }
            context.Students.Local[0].School = context.Schools.Local[0];
            context.Students.Local[11].School = context.Schools.Local[1];
            context.Students.Local[12].School = context.Schools.Local[0];
            context.Students.Local[3].FirstName = context.Students.Local[3].FirstName + ", Jr.";
            context.Students.Local[5].MiddleName = context.Students.Local[5].MiddleName + ", Microsoft";
            context.Students.Local[7].LastName = context.Students.Local[7].LastName + ", III";
            context.Students.Add(new Student
            {
                FirstName = "Student01",
                LastName = "Automation",
                School = context.Schools.Local.Single(s => s.Name == "Local High School"),
                Grade = 12,
                StudentSISId = "9999990"
            });
            context.Students.Add(new Student
            {
                FirstName = "Student02",
                LastName = "Automation",
                School = context.Schools.Local.Single(s => s.Name == "Local High School"),
                Grade = 11,
                StudentSISId = "9999991"
            });
            context.Students.Add(new Student
            {
                FirstName = "Student03",
                LastName = "Automation",
                School = context.Schools.Local.Single(s => s.Name == "Local High School"),
                Grade = 10,
                StudentSISId = "9999992"
            });
            context.Students.Add(new Student
            {
                FirstName = "Student04",
                LastName = "Automation",
                School = context.Schools.Local.Single(s => s.Name == "Community Middle School"),
                Grade = 9,
                StudentSISId = "9999993"
            });
            context.Students.Add(new Student
            {
                FirstName = "Student05",
                LastName = "Automation",
                School = context.Schools.Local.Single(s => s.Name == "Community Middle School"),
                Grade = 8,
                StudentSISId = "9999994"
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeCategories(EducationDataContext context)
        {
            context.Categories.Add(new Category { Name = "Test Category," });
        }

        [Conditional("DEBUG")]
        private static void InitializeServiceTypes(EducationDataContext context)
        {
            context.ServiceTypes.Add(new ServiceType
            {
                Name = "Provide College Access",
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Categories = new List<Category> { context.Categories.Find(1) },
                IsActive = true
            });
            context.ServiceTypes.Add(new ServiceType
            {
                Name = "Mentoring",
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Categories = new List<Category> { context.Categories.Find(4), context.Categories.Find(9) },
                IsActive = true
            });
            context.ServiceTypes.Add(new ServiceType
            {
                Name = "Test service typ,e",
                Description = "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                Categories = new List<Category> { context.Categories.Local[0] },
                IsActive = true
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeStudentAssignedOfferings(EducationDataContext context)
        {
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[0],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                CreateTime = DateTime.Now,
                Notes = "This is a test note",
                Student = context.Students.Local[1],
                CreatingUser = context.Users.Local[0],
                IsActive = true
            });
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[0],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                CreateTime = DateTime.Now,
                Notes = "This is a test note",
                Student = context.Students.Local[3],
                CreatingUser = context.Users.Local[1],
                IsActive = true
            });
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[1],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                Notes = "This is a test note",
                Student = context.Students.Local[0],
                CreatingUser = context.Users.Local[0],
                IsActive = true
            });
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[1],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                Notes = "This is a test note",
                Student = context.Students.Local[2],
                CreatingUser = context.Users.Local[1],
                IsActive = true
            });
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[2],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                Notes = "This is a test note",
                Student = context.Students.Local[4],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-20),
                LastModifyingUser = context.Users.Local[0],
                LastModifyTime = DateTime.Now.AddDays(-10),
                IsActive = true
            });
            context.StudentAssignedOfferings.Add(new StudentAssignedOffering
            {
                ServiceOffering = context.ServiceOfferings.Local[1],
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(30),
                Notes = "This is a test note",
                Student = context.Students.Local[6],
                CreatingUser = context.Users.Local[0],
                IsActive = false
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeServiceAttendances(EducationDataContext context)
        {
            context.ServiceAttendances.Add(new ServiceAttendance
            {
                StudentAssignedOffering = context.StudentAssignedOfferings.Local[0],
                DateAttended = DateTime.Now.AddDays(-2),
                Subject = context.Subjects.Local[0],
                Duration = 30,
                Notes = "This is a test note",
                CreateTime = DateTime.Now.AddDays(-1),
                CreatingUser = context.Users.Local[0]
            });
            context.ServiceAttendances.Add(new ServiceAttendance
            {
                StudentAssignedOffering = context.StudentAssignedOfferings.Local[0],
                DateAttended = DateTime.Now.AddDays(-4),
                Subject = context.Subjects.Local[1],
                Duration = 30,
                Notes = "This is another note",
                CreateTime = DateTime.Now.AddDays(-3),
                CreatingUser = context.Users.Local[1],
                LastModifyTime = DateTime.Now.AddDays(-2),
                LastModifyingUser = context.Users.Local[1]
            });
        }

        [Conditional("DEBUG")]
        private static void InitializePriorities(EducationDataContext context)
        {
            context.Priorities.Add(new Priority { Name = "Super, Duper High" });
        }

        [Conditional("DEBUG")]
        private static void InitializeSubjects(EducationDataContext context)
        {
            context.Subjects.Add(new Subject { Name = "Hard, Hard Math" });
        }

        [Conditional("DEBUG")]
        private static void InitializeServiceRequests(EducationDataContext context)
        {
            context.ServiceRequests.Add(new ServiceRequest
            {
                Student = context.Students.Local[0],
                ServiceType = context.ServiceTypes.Local[0],
                Priority = context.Priorities.Find(1),
                Subject = context.Subjects.Find(1),
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-40)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Open),
                ServiceRequest = context.ServiceRequests.Local[0],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-40)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Rejected),
                Notes = "Student's parent do not approve of this service type.",
                ServiceRequest = context.ServiceRequests.Local[0],
                CreatingUser = context.Users.Local[1],
                CreateTime = DateTime.Now.AddDays(-20)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfilledBy = context.StudentAssignedOfferings.Local[1],
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Fulfilled),
                Notes = "Verified student is actually already receiving this service offering.",
                ServiceRequest = context.ServiceRequests.Local[0],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-10)
            });
            context.ServiceRequests.Add(new ServiceRequest
            {
                Student = context.Students.Local[1],
                ServiceType = context.ServiceTypes.Local[2],
                Priority = context.Priorities.Find(3),
                Subject = context.Subjects.Find(3),
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-70)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Open),
                ServiceRequest = context.ServiceRequests.Local[1],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-70)
            });
            context.ServiceRequests.Add(new ServiceRequest
            {
                Student = context.Students.Local[5],
                ServiceType = context.ServiceTypes.Local[0],
                Priority = context.Priorities.Find(2),
                Subject = context.Subjects.Find(2),
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-80)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfilledBy = context.StudentAssignedOfferings.Local[0],
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Open),
                ServiceRequest = context.ServiceRequests.Local[2],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-80)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfilledBy = context.StudentAssignedOfferings.Local[0],
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Rejected),
                Notes = "Student's parent do not approve of this service type.",
                ServiceRequest = context.ServiceRequests.Local[2],
                CreatingUser = context.Users.Local[0]
            });
            context.ServiceRequests.Add(new ServiceRequest
            {
                Student = context.Students.Local[7],
                ServiceType = context.ServiceTypes.Local[1],
                Priority = context.Priorities.Find(4),
                Subject = context.Subjects.Find(5),
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-45)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Open),
                ServiceRequest = context.ServiceRequests.Local[3],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-45)
            });
            context.ServiceRequests.Add(new ServiceRequest
            {
                Student = context.Students.Local[8],
                ServiceType = context.ServiceTypes.Local[1],
                Priority = context.Priorities.Find(2),
                Subject = context.Subjects.Find(4),
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-33),
                LastModifyingUser = context.Users.Local[1],
                LastModifyTime = DateTime.Now.AddDays(-29)
            });
            context.ServiceRequestFulfillments.Add(new ServiceRequestFulfillment
            {
                FulfillmentStatus = context.FulfillmentStatuses.Single(s => s.Name == Statuses.Open),
                ServiceRequest = context.ServiceRequests.Local[4],
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-33)
            });
        }

        [Conditional("DEBUG")]
        private void InitializeCustomDataOrigin(EducationDataContext context)
        {
            context.CustomDataOrigins.Add(new CustomDataOrigin
            {
                FileName = "testfile.csv",
                CreateTime = new DateTime(2012, 12, 21),
                AzureBlobKey = "testingkey", 
                CreatingUser = context.Users.Local[0],
                CreatingUserId = context.Users.Local[0].Id,
                Source = "College Board",
                WasManualEntry = false
            });
            context.CustomDataOrigins.Add(new CustomDataOrigin
            {
                CreateTime = new DateTime(2011, 10, 10),
                AzureBlobKey = "testingkey2", 
                CreatingUserId = context.Users.Local[1].Id,
                CreatingUser = context.Users.Local[1],
                Source = "Checkers",
                WasManualEntry = true
            });
        }

        [Conditional("DEBUG")]
        private void InitializeCustomFieldValues(EducationDataContext context)
        {
            context.CustomFieldValues.Add(new CustomFieldValue
            {
                CustomField = context.CustomFields.Local[0],
                CustomDataOrigin = context.CustomDataOrigins.Local[0],
                Student = context.Students.Local[0],
                Value = "1200"
            });
            context.CustomFieldValues.Add(new CustomFieldValue
            {
                CustomField = context.CustomFields.Local[1],
                CustomDataOrigin = context.CustomDataOrigins.Local[0],
                Student = context.Students.Local[0],
                Value = "1201",
            });
            context.CustomFieldValues.Add(new CustomFieldValue
            {
                CustomField = context.CustomFields.Local[2],
                CustomDataOrigin = context.CustomDataOrigins.Local[0],
                Student = context.Students.Local[0],
                Value = "1202",
            });
            context.Students.Local[0].CustomFieldValues = new List<CustomFieldValue>
            {
                context.CustomFieldValues.Local[0],
                context.CustomFieldValues.Local[1],
                context.CustomFieldValues.Local[2]
            };
        }

        [Conditional("DEBUG")]
        private void InitializeCustomField(EducationDataContext context)
        {
            context.CustomFields.Add(new PublicField
            {
                CustomFieldType = context.CustomFieldTypes.Find(1),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(1), context.CustomFieldCategories.Find(2) },
                Name = "Tardies",
                CreatingUser = context.Users.Local[0],
                CreateTime = DateTime.Now.AddDays(-30),
                LastModifyingUser = context.Users.Local[0],
                LastModifyTime = DateTime.Now.AddDays(-10)
            });
            context.CustomFieldCategories.Find(1).Fields.Add(context.CustomFields.Local[0]);
            context.CustomFieldCategories.Find(2).Fields.Add(context.CustomFields.Local[0]);
            context.CustomFields.Add(new PublicField
            {
                CustomFieldType = context.CustomFieldTypes.Find(1),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(4) },
                Name = "ACT",
                CreatingUser = context.Users.Local[0]
            });
            context.CustomFieldCategories.Find(4).Fields.Add(context.CustomFields.Local[1]);
            context.CustomFields.Add(new PublicField
            {
                CustomFieldType = context.CustomFieldTypes.Find(1),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(4) },
                Name = "SAT",
                CreatingUser = context.Users.Local[0]
            });
            context.CustomFieldCategories.Find(4).Fields.Add(context.CustomFields.Local[2]);
            context.CustomFields.Add(new PublicField
            {
                CustomFieldType = context.CustomFieldTypes.Find(2),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(3) },
                Name = "Guardian",
                CreatingUser = context.Users.Local[0]
            });
            context.CustomFieldCategories.Find(3).Fields.Add(context.CustomFields.Local[3]);
            context.CustomFields.Add(new PrivateHealthField
            {
                CustomFieldType = context.CustomFieldTypes.Find(2),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(2) },
                Name = "FEE",
                CreatingUser = context.Users.Local[0],
                Provider = context.Providers.Local[1]
            });
            context.CustomFields.Add(new PrivateHealthField
            {
                CustomFieldType = context.CustomFieldTypes.Find(3),
                Categories = new List<CustomFieldCategory> { context.CustomFieldCategories.Find(3) },
                Name = "PHI",
                CreatingUser = context.Users.Local[0]
            });
        }

        [Conditional("DEBUG")]
        private static void InitializeUsers(EducationDataContext context)
        {
            User user = new User
            {
                UserKey = "Bob",
                DisplayName = "Bob",
                EmailAddress = "Bob@bob.bob",
                FirstName = "Bob",
                LastName = "Bob",
                CreateTime = new DateTime(2000, 1, 1)
            };
            user.EulaAcceptances.Add(new EulaAcceptance { CreateTime = DateTime.Now, CreatingUser = user, CreatingUserId = user.Id, EulaAgreement = context.EulaAgreements.OrderByDescending(e => e.CreateTime).First() });
            context.Users.Add(user);
            user.UserRoles = new List<UserRole>
            {
                new UserRole 
                {
                    User = user,
                    Role = context.Roles.Find(1),
                    CreateTime = new DateTime(2000, 1, 1),
                    CreatingUser = user
                }
            };
            context.Roles.Local[0].UserRoles.Add(context.Users.Local[0].UserRoles.First());
            user.FavoriteServiceOfferings = new List<ServiceOffering>
            {
                context.ServiceOfferings.Local[1]
            };
            context.ServiceOfferings.Local[1].UsersLinkingAsFavorite.Add(user);
            user = new User
            {
                UserKey = "Jim",
                DisplayName = "Jim",
                EmailAddress = "Jim@jim.jim",
                FirstName = "Jim",
                LastName = "Jim",
                CreateTime = new DateTime(2000, 1, 1)
            };
            context.Users.Add(user);
            user.UserRoles = new List<UserRole>
            {
                new UserRole 
                {
                    User = user,
                    Role = context.Roles.Find(2),
                    CreateTime = new DateTime(2000, 1, 1),
                    CreatingUser = context.Users.Local[0],
                    Schools = new List<School> { context.Schools.Local[0] }
                }
            };
            context.Roles.Local[1].UserRoles.Add(context.Users.Local[1].UserRoles.First());
            user = new User
            {
                UserKey = "Fred",
                DisplayName = "Fred",
                EmailAddress = "Fred@fred.fred",
                FirstName = "Fred",
                LastName = "Fred",
                CreateTime = new DateTime(2000, 1, 1)
            };
            context.Users.Add(user);
            user.UserRoles = new List<UserRole>
            {
                new UserRole 
                {
                    User = user,
                    Role = context.Roles.Find(3),
                    CreateTime = new DateTime(2000, 1, 1),
                    CreatingUser = context.Users.Local[0],
                    Providers = new List<Provider> { context.Providers.Local[1] }
                }
            };
            context.Roles.Local[1].UserRoles.Add(context.Users.Local[1].UserRoles.First());
        }
    }
}
