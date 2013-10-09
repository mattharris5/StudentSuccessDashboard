using EFCachingProvider.Caching;
using EFTracingProvider;
using SSD.Domain;
using System;
using System.Data.Entity;
using System.IO;

namespace SSD.Data
{
    public interface IEducationContext
    {
        event EventHandler<CommandExecutionEventArgs> CommandExecuting;
        event EventHandler<CommandExecutionEventArgs> CommandFinished;
        event EventHandler<CommandExecutionEventArgs> CommandFailed;

        IDbSet<Category> Categories { get; set; }
        IDbSet<Class> Classes { get; set; }
        IDbSet<CustomDataOrigin> CustomDataOrigins { get; set; }
        IDbSet<CustomFieldCategory> CustomFieldCategories { get; set; }
        IDbSet<CustomFieldValue> CustomFieldValues { get; set; }
        IDbSet<CustomField> CustomFields { get; set; }
        IDbSet<CustomFieldType> CustomFieldTypes { get; set; }
        IDbSet<EulaAcceptance> EulaAcceptances { get; set; }
        IDbSet<EulaAgreement> EulaAgreements { get; set; }
        IDbSet<Priority> Priorities { get; set; }
        IDbSet<PrivateHealthDataViewEvent> PrivateHealthDataViewEvents { get; set; }
        IDbSet<Property> Properties { get; set; }
        IDbSet<Provider> Providers { get; set; }
        IDbSet<Program> Programs { get; set; }
        IDbSet<Role> Roles { get; set; }
        IDbSet<School> Schools { get; set; }
        IDbSet<ServiceOffering> ServiceOfferings { get; set; }
        IDbSet<ServiceRequest> ServiceRequests { get; set; }
        IDbSet<FulfillmentStatus> FulfillmentStatuses { get; set; }
        IDbSet<ServiceRequestFulfillment> ServiceRequestFulfillments { get; set; }
        IDbSet<ServiceType> ServiceTypes { get; set; }
        IDbSet<StudentAssignedOffering> StudentAssignedOfferings { get; set; }
        IDbSet<ServiceAttendance> ServiceAttendances { get; set; }
        IDbSet<Student> Students { get; set; }
        IDbSet<Subject> Subjects { get; set; }
        IDbSet<Teacher> Teachers { get; set; }
        IDbSet<UserRole> UserRoles { get; set; }
        IDbSet<User> Users { get; set; }
        IDbSet<UserAccessChangeEvent> UserAccessChangeEvents { get; set; }
        IDbSet<LoginEvent> LoginEvents { get; set; }

        ICache Cache { get; set; }
        CachingPolicy CachingPolicy { get; set; }
        TextWriter Log { get; set; }

        void SetModified(object entity);
        int SaveChanges();
    }
}
