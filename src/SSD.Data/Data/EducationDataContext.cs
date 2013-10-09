using EFCachingProvider;
using EFCachingProvider.Caching;
using EFProviderWrapperToolkit;
using EFTracingProvider;
using Microsoft.WindowsAzure;
using SSD.Domain;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace SSD.Data
{
    public class EducationDataContext : DbContext, IEducationContext
    {
        public const string DatabaseConnectionStringSettingName = "DatabaseConnectionString";
        private const string WrappedProvider = "System.Data.SqlClient";
        private static readonly string[] WrapperProviders = new[] { "EFTracingProvider", "EFCachingProvider" };

        private TextWriter _log;
        
        static EducationDataContext()
        {
            EFTracingProviderConfiguration.DefaultWrappedProvider = WrappedProvider;
            EFTracingProviderConfiguration.RegisterProvider();
            EFCachingProviderConfiguration.DefaultWrappedProvider = WrappedProvider;
            EFCachingProviderConfiguration.RegisterProvider();
        }

        public EducationDataContext()
            : base(CreateConnection(CloudConfigurationManager.GetSetting(DatabaseConnectionStringSettingName), WrappedProvider), true)
        {
            Database.SetInitializer(new EducationDatabaseInitializer());
        }

        private static DbConnection CreateConnection(string nameOrConnectionString, string providerInvariantName)
        {
            string connectionString = NormalizeConnectionString(nameOrConnectionString);
            DbConnection connection = DbProviderFactories.GetFactory(providerInvariantName).CreateConnection();
            connection.ConnectionString = connectionString;
            connection = DbConnectionWrapper.WrapConnection(providerInvariantName, connection, WrapperProviders);
            return connection;
        }

        public static string NormalizeConnectionString(string nameOrConnectionString)
        {
            string connectionString = ResolveConnectionString(nameOrConnectionString);
            return string.Join(";", connectionString.Split(';').Where(s => !s.StartsWith("wrappedProvider=")));
        }

        private static string ResolveConnectionString(string nameOrConnectionString)
        {
            var setting = ConfigurationManager.ConnectionStrings[nameOrConnectionString];
            if (setting == null)
            {
                return nameOrConnectionString;
            }
            return setting.ConnectionString;
        }

        private EFTracingConnection TracingConnection
        {
            get { return this.AsObjectContext().UnwrapConnection<EFTracingConnection>(); }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandExecuting
        {
            add { this.TracingConnection.CommandExecuting += value; }
            remove { this.TracingConnection.CommandExecuting -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFinished
        {
            add { this.TracingConnection.CommandFinished += value; }
            remove { this.TracingConnection.CommandFinished -= value; }
        }

        public event EventHandler<CommandExecutionEventArgs> CommandFailed
        {
            add { this.TracingConnection.CommandFailed += value; }
            remove { this.TracingConnection.CommandFailed -= value; }
        }

        private void OnCommandFinished(object sender, CommandExecutionEventArgs e)
        {
            if (_log != null)
            {
                _log.WriteLine(string.Format("#{0} Command completed in {1}", e.CommandId, e.Duration), "Information");
            }
        }

        private void OnCommandFailed(object sender, CommandExecutionEventArgs e)
        {
            if (_log != null)
            {
                _log.WriteLine(string.Format("#{0} Command failed {1}", e.CommandId, e.Result), "Error");
            }
        }

        private void OnCommandExecuting(object sender, CommandExecutionEventArgs e)
        {
            if (_log != null)
            {
                _log.WriteLine(string.Format("#{0} Running {1}:\r\n{2}", e.CommandId, e.Method.Trim(), e.ToTraceString().Trim()), "Information");
            }
        }

        public TextWriter Log
        {
            get { return _log; }
            set
            {
                if ((_log != null) != (value != null))
                {
                    if (value == null)
                    {
                        CommandExecuting -= OnCommandExecuting;
                        CommandFailed -= OnCommandFailed;
                        CommandFinished -= OnCommandFinished;
                    }
                    else
                    {
                        CommandExecuting += OnCommandExecuting;
                        CommandFailed += OnCommandFailed;
                        CommandFinished += OnCommandFinished;
                    }
                }
                _log = value;
            }
        }

        private EFCachingConnection CachingConnection
        {
            get { return this.AsObjectContext().UnwrapConnection<EFCachingConnection>(); }
        }

        public ICache Cache
        {
            get { return CachingConnection.Cache; }
            set { CachingConnection.Cache = value; }
        }

        public CachingPolicy CachingPolicy
        {
            get { return CachingConnection.CachingPolicy; }
            set { CachingConnection.CachingPolicy = value; }
        }

        public void SetModified(object entity)
        {
            DbEntityEntry entry = this.Entry(entity);
            entry.State = EntityState.Modified;
        }

        public IDbSet<CustomFieldCategory> CustomFieldCategories { get; set; }
        public IDbSet<CustomFieldType> CustomFieldTypes { get; set; }
        public IDbSet<CustomField> CustomFields { get; set; }
        public IDbSet<CustomDataOrigin> CustomDataOrigins { get; set; }
        public IDbSet<CustomFieldValue> CustomFieldValues { get; set; }
        public IDbSet<EulaAcceptance> EulaAcceptances { get; set; }
        public IDbSet<EulaAgreement> EulaAgreements { get; set; }
        public IDbSet<Student> Students { get; set; }
        public IDbSet<Class> Classes { get; set; }
        public IDbSet<Teacher> Teachers { get; set; }
        public IDbSet<Provider> Providers { get; set; }
        public IDbSet<Program> Programs { get; set; }
        public IDbSet<School> Schools { get; set; }
        public IDbSet<Category> Categories { get; set; }
        public IDbSet<ServiceType> ServiceTypes { get; set; }
        public IDbSet<ServiceOffering> ServiceOfferings { get; set; }
        public IDbSet<ServiceRequest> ServiceRequests { get; set; }
        public IDbSet<FulfillmentStatus> FulfillmentStatuses { get; set; }
        public IDbSet<ServiceRequestFulfillment> ServiceRequestFulfillments { get; set; }
        public IDbSet<User> Users { get; set; }
        public IDbSet<Role> Roles { get; set; }
        public IDbSet<UserRole> UserRoles { get; set; }
        public IDbSet<Priority> Priorities { get; set; }
        public IDbSet<Subject> Subjects { get; set; }
        public IDbSet<Property> Properties { get; set; }
        public IDbSet<StudentAssignedOffering> StudentAssignedOfferings { get; set; }
        public IDbSet<ServiceAttendance> ServiceAttendances { get; set; }
        public IDbSet<UserAccessChangeEvent> UserAccessChangeEvents { get; set; }
        public IDbSet<PrivateHealthDataViewEvent> PrivateHealthDataViewEvents { get; set; }
        public IDbSet<LoginEvent> LoginEvents { get; set; }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                StringBuilder message = new StringBuilder(string.Format(CultureInfo.InvariantCulture, "Could not save {0} data due to error.  {1}", GetType().Name, e.ToString()));
                foreach (var validation in e.EntityValidationErrors)
                {
                    message.Append("Validation Errors for Entity: ");
                    message.AppendLine(validation.Entry.ToString());
                    foreach (var errorMessage in validation.ValidationErrors)
                    {
                        message.AppendLine(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", errorMessage.PropertyName, errorMessage.ErrorMessage));
                    }
                }
                Trace.WriteLine(message.ToString(), "Error");
                throw;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }
            base.OnModelCreating(modelBuilder);
            CreateApplicationSchema(modelBuilder);
        }

        private static void CreateApplicationSchema(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Priority>().ToTable("Priority", "SSD");
            modelBuilder.Entity<Priority>().Property(p => p.Id).HasColumnName("PriorityId");
            modelBuilder.Entity<Subject>().ToTable("Subject", "SSD");
            modelBuilder.Entity<Subject>().Property(s => s.Id).HasColumnName("SubjectId");
            modelBuilder.Entity<User>().ToTable("User", "SSD");
            modelBuilder.Entity<User>().Property(u => u.Id).HasColumnName("UserId");
            modelBuilder.Entity<User>().Property(u => u.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<LoginEvent>().ToTable("LoginEvent", "SSD");
            modelBuilder.Entity<LoginEvent>().Property(u => u.Id).HasColumnName("LoginEventId");
            modelBuilder.Entity<LoginEvent>().Property(u => u.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<Role>().ToTable("Role", "SSD");
            modelBuilder.Entity<Role>().Property(r => r.Id).HasColumnName("RoleId");
            modelBuilder.Entity<Provider>().ToTable("Provider", "SSD");
            modelBuilder.Entity<Provider>().Property(p => p.Id).HasColumnName("ProviderId");
            modelBuilder.Entity<Program>().ToTable("Program", "SSD");
            modelBuilder.Entity<Program>().Property(p => p.Id).HasColumnName("ProgramId");
            modelBuilder.Entity<Program>().
                HasMany(s => s.Schools).
                WithMany(c => c.Programs).
                Map(m =>
                {
                    m.MapLeftKey("ProgramId");
                    m.MapRightKey("SchoolId");
                    m.ToTable("ProgramSchools", "SSD");
                });
            modelBuilder.Entity<School>().ToTable("School", "SSD");
            modelBuilder.Entity<School>().Property(s => s.Id).HasColumnName("SchoolId");
            modelBuilder.Entity<Teacher>().ToTable("Teacher", "SSD");
            modelBuilder.Entity<Teacher>().Property(t => t.Id).HasColumnName("TeacherId");
            modelBuilder.Entity<Class>().ToTable("Class", "SSD");
            modelBuilder.Entity<Class>().Property(c => c.Id).HasColumnName("ClassId");
            modelBuilder.Entity<Student>().ToTable("Student", "SSD");
            modelBuilder.Entity<Student>().Property(s => s.Id).HasColumnName("StudentId");
            modelBuilder.Entity<Student>().
                HasMany(s => s.Classes).
                WithMany(c => c.Students).
                Map(m =>
                {
                    m.MapLeftKey("StudentId");
                    m.MapRightKey("ClassId");
                    m.ToTable("StudentClasses", "SSD");
                });
            modelBuilder.Entity<Student>().
                HasMany(s => s.ApprovedProviders).
                WithMany(p => p.ApprovingStudents).
                Map(m =>
                {
                    m.MapLeftKey("StudentId");
                    m.MapRightKey("ProviderId");
                    m.ToTable("StudentProviders", "SSD");
                });
            modelBuilder.Entity<Category>().ToTable("Category", "SSD");
            modelBuilder.Entity<Category>().Property(c => c.Id).HasColumnName("CategoryId");
            modelBuilder.Entity<ServiceType>().ToTable("ServiceType", "SSD");
            modelBuilder.Entity<ServiceType>().Property(s => s.Id).HasColumnName("ServiceTypeId");
            modelBuilder.Entity<ServiceType>().
                HasMany(s => s.Categories).
                WithMany(c => c.ServiceTypes).
                Map(m =>
                {
                    m.MapLeftKey("ServiceTypeId");
                    m.MapRightKey("CategoryId");
                    m.ToTable("ServiceTypeCategories", "SSD");
                });
            modelBuilder.Entity<ServiceOffering>().ToTable("ServiceOffering", "SSD");
            modelBuilder.Entity<ServiceOffering>().Property(m => m.Id).HasColumnName("ServiceOfferingId");
            modelBuilder.Entity<ServiceOffering>().
                HasMany(m => m.UsersLinkingAsFavorite).
                WithMany(m => m.FavoriteServiceOfferings).
                Map(m =>
                {
                    m.MapLeftKey("ServiceOfferingId");
                    m.MapRightKey("UserId");
                    m.ToTable("FavoriteServiceOffering", "SSD");
                });
            modelBuilder.Entity<StudentAssignedOffering>().ToTable("StudentAssignedOffering", "SSD");
            modelBuilder.Entity<StudentAssignedOffering>().Property(a => a.Id).HasColumnName("StudentAssignedOfferingId");
            modelBuilder.Entity<StudentAssignedOffering>().Property(a => a.StartDate).HasColumnType("datetime2");
            modelBuilder.Entity<StudentAssignedOffering>().Property(a => a.EndDate).HasColumnType("datetime2");
            modelBuilder.Entity<StudentAssignedOffering>().Property(c => c.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<ServiceAttendance>().ToTable("ServiceAttendance", "SSD");
            modelBuilder.Entity<ServiceAttendance>().Property(a => a.Id).HasColumnName("ServiceAttendanceId");
            modelBuilder.Entity<ServiceAttendance>().Property(c => c.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<ServiceAttendance>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.CreatedServiceAttendances).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<ServiceRequest>().ToTable("ServiceRequest", "SSD");
            modelBuilder.Entity<ServiceRequest>().Property(s => s.Id).HasColumnName("ServiceRequestId");
            modelBuilder.Entity<ServiceRequest>().Property(s => s.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<FulfillmentStatus>().ToTable("FulfillmentStatus", "SSD");
            modelBuilder.Entity<FulfillmentStatus>().Property(s => s.Id).HasColumnName("FulfillmentStatusId");
            modelBuilder.Entity<ServiceRequestFulfillment>().ToTable("ServiceRequestFulfillment", "SSD");
            modelBuilder.Entity<ServiceRequestFulfillment>().Property(s => s.Id).HasColumnName("ServiceRequestFulfillmentId");
            modelBuilder.Entity<ServiceRequestFulfillment>().Property(s => s.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<ServiceRequestFulfillment>().
                HasOptional(f => f.FulfilledBy).
                WithMany(a => a.Fulfillments).
                HasForeignKey(a => a.FulfilledById).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<ServiceRequestFulfillment>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.CreatedServiceRequestFulfillments).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<UserRole>().ToTable("UserRoles", "SSD");
            modelBuilder.Entity<UserRole>().Property(u => u.Id).HasColumnName("UserRoleId");
            modelBuilder.Entity<UserRole>().Property(u => u.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<UserRole>().
                HasMany(u => u.Schools).
                WithMany(s => s.UserRoles).
                Map(m =>
                {
                    m.MapLeftKey("UserRoleId");
                    m.MapRightKey("SchoolId");
                    m.ToTable("UserRoleSchools", "SSD");
                });
            modelBuilder.Entity<UserRole>().
                HasMany(u => u.Providers).
                WithMany(p => p.UserRoles).
                Map(m =>
                {
                    m.MapLeftKey("UserRoleId");
                    m.MapRightKey("ProviderId");
                    m.ToTable("UserRoleProviders", "SSD");
                });
            modelBuilder.Entity<UserRole>().
                HasRequired(f => f.User).
                WithMany(u => u.UserRoles).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<UserRole>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.CreatedUserRoles).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<UserAccessChangeEvent>().ToTable("UserAccessChangeEvent", "SSD");
            modelBuilder.Entity<UserAccessChangeEvent>().Property(a => a.AccessData).HasColumnType("xml");
            modelBuilder.Entity<UserAccessChangeEvent>().Property(a => a.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<UserAccessChangeEvent>().
                HasRequired(f => f.User).
                WithMany(u => u.UserAccessChangeEvents).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<UserAccessChangeEvent>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.CreatedUserAccessChangeEvents).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<PrivateHealthDataViewEvent>().ToTable("PrivateHealthInfoViewEvent", "SSD");
            modelBuilder.Entity<PrivateHealthDataViewEvent>().Property(a => a.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<PrivateHealthDataViewEvent>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.PrivateHealthDataViewEvents).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<PrivateHealthDataViewEvent>().
                HasMany(p => p.PhiValuesViewed).
                WithMany(c => c.PrivateHealthDataViewEvents).
                Map(m =>
                {
                    m.MapLeftKey("PrivateHealthDataViewEventId");
                    m.MapRightKey("CustomFieldValueId");
                    m.ToTable("PrivateHealthDataViewEventsCustomFieldValues", "SSD");
                });
            modelBuilder.Entity<Property>().ToTable("Property", "SSD");
            modelBuilder.Entity<Property>().Property(p => p.Id).HasColumnName("PropertyId");
            modelBuilder.Entity<CustomFieldCategory>().ToTable("CustomFieldCategory", "SSD");
            modelBuilder.Entity<CustomFieldCategory>().Property(c => c.Id).HasColumnName("CustomFieldCategoryId");
            modelBuilder.Entity<CustomFieldType>().ToTable("CustomFieldType", "SSD");
            modelBuilder.Entity<CustomFieldType>().Property(c => c.Id).HasColumnName("CustomFieldTypeId");
            modelBuilder.Entity<CustomField>().ToTable("CustomField", "SSD");
            modelBuilder.Entity<CustomField>().Property(c => c.Id).HasColumnName("CustomFieldId");
            modelBuilder.Entity<CustomField>().Property(c => c.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<CustomField>().
                HasRequired(c => c.CreatingUser).
                WithMany(u => u.CreatedCustomFields).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomField>().HasMany(c => c.Categories).WithMany(c => c.Fields).
                Map(m => 
                {
                    m.MapLeftKey("CustomFieldId");
                    m.MapRightKey("CustomFieldCategoryId");
                    m.ToTable("CustomFieldCategoryMapping", "SSD");
                });
            modelBuilder.Entity<CustomDataOrigin>().ToTable("CustomDataOrigin", "SSD");
            modelBuilder.Entity<CustomDataOrigin>().Property(c => c.Id).HasColumnName("CustomDataOriginId");
            modelBuilder.Entity<CustomDataOrigin>().Property(c => c.CreateTime).HasColumnType("datetime2");
            modelBuilder.Entity<CustomDataOrigin>().
                HasRequired(c => c.CreatingUser).
                WithMany(u => u.CreatedCustomDataOrigins).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<CustomFieldValue>().ToTable("CustomFieldValue", "SSD");
            modelBuilder.Entity<CustomFieldValue>().Property(c => c.Id).HasColumnName("CustomFieldValueId");
            modelBuilder.Entity<EulaAgreement>().ToTable("EulaAgreement", "SSD");
            modelBuilder.Entity<EulaAgreement>().Property(c => c.Id).HasColumnName("EulaAgreementId");
            modelBuilder.Entity<EulaAgreement>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.CreatedEulaAgreements).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<EulaAcceptance>().ToTable("EulaAcceptance", "SSD");
            modelBuilder.Entity<EulaAcceptance>().Property(c => c.Id).HasColumnName("EulaAcceptanceId");
            modelBuilder.Entity<EulaAcceptance>().
                HasRequired(f => f.CreatingUser).
                WithMany(u => u.EulaAcceptances).
                WillCascadeOnDelete(false);
            modelBuilder.Entity<PrivateHealthField>().ToTable("PrivateHealthField", "SSD");
            modelBuilder.Entity<PrivateHealthField>().Property(p => p.Id).HasColumnName("PrivateHealthFieldId");
            modelBuilder.Entity<PublicField>().ToTable("PublicField", "SSD");
            modelBuilder.Entity<PublicField>().Property(p => p.Id).HasColumnName("PublicFieldId");
        }
    }
}