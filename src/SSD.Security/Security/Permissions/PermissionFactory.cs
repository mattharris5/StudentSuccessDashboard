using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SSD.Security.Permissions
{
    public class PermissionFactory : IPermissionFactory
    {
        private static readonly IReadOnlyDictionary<string, Type> Configuration = new ReadOnlyDictionary<string, Type>(new Dictionary<string, Type>
        {
            { "ViewStudentDetail", typeof(ViewStudentDetailPermission) },
            { "CreateStudentNameString", typeof(ViewStudentDetailPermission) },
            { "StudentProfileExportMapData", typeof(ViewStudentDetailPermission) },
            { "SetServiceTypePrivacy", typeof(SetServiceTypePrivacyPermission) },
            { "EditRequest", typeof(ManageServiceRequestPermission) },
            { "DeleteRequest", typeof(ManageServiceRequestPermission) },
            { "CreateServiceRequestString", typeof(ManageServiceRequestPermission) },
            { "EditScheduledOffering", typeof(ManageAssignedOfferingPermission) },
            { "DeleteScheduledOffering", typeof(ManageAssignedOfferingPermission) },
            { "EditProvider", typeof(ManageProviderPermission) },
            { "SetFavoriteServiceOffering", typeof(ManageProviderPermission) },
            { "CreateServiceRequest", typeof(CreateServiceRequestPermission) },
            { "ProcessDataFile", typeof(ManageCustomFieldPermission) },
            { "CreateServiceAttendance", typeof(ManageServiceAttendancePermission) },
            { "EditServiceAttendance", typeof(ManageServiceAttendancePermission) },
            { "DeleteServiceAttendance", typeof(ManageServiceAttendancePermission) },
            { "ScheduleOffering", typeof(ScheduleOfferingPermission) },
            { "ImportOfferingData", typeof(ImportOfferingDataPermission) },
            { "UploadCustomFieldData", typeof(CustomFieldDataPermission) },
            { "ViewStudentCustomFieldData", typeof(CustomFieldDataPermission) },
            { "StudentProfileExportCustomFieldData", typeof(CustomFieldDataPermission) }
        });

        private static IPermissionFactory _instance = new PermissionFactory();

        public static IPermissionFactory Current
        {
            get { return _instance; }
        }

        public static void SetCurrent(IPermissionFactory current)
        {
            if (current == null)
            {
                throw new ArgumentNullException("current");
            }
            _instance = current;
        }

        public IPermission Create(string activity, params object[] args)
        {
            if (string.IsNullOrWhiteSpace(activity))
            {
                throw new ArgumentNullException("activity");
            }
            if (Configuration.ContainsKey(activity))
            {
                return (IPermission)Activator.CreateInstance(Configuration[activity], args);
            }
            throw new InvalidOperationException("Specified activity unrecognized.");
        }
    }
}
