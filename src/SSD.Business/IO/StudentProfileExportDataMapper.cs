using SSD.Domain;
using SSD.Security;
using SSD.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SSD.IO
{
    public class StudentProfileExportDataMapper : IExportDataMapper
    {
        private static readonly IReadOnlyCollection<string> _RequiredColumnHeadings = new ReadOnlyCollection<string>(new List<string>
        {
            "School Description",
            "Grade",
            "Student Name",
            "Student ID"
        });

        public IEnumerable<string> MapColumnHeadings(StudentProfileExportFieldDescriptor fieldConfiguration)
        {
            List<string> columnHeadings = new List<string>(_RequiredColumnHeadings);
            MapStandardFieldHeadings(fieldConfiguration, columnHeadings);
            columnHeadings.AddRange(fieldConfiguration.SelectedCustomFields.Select(f => f.Name));
            columnHeadings.AddRange(fieldConfiguration.SelectedServiceTypes.Select(t => t.Name));
            return columnHeadings;
        }

        IEnumerable<string> IExportDataMapper.MapColumnHeadings(object descriptor)
        {
            return MapColumnHeadings((StudentProfileExportFieldDescriptor)descriptor);
        }

        public IEnumerable<object> MapData(StudentProfileExportFieldDescriptor fieldConfiguration, Student data, EducationSecurityPrincipal user, IUserAuditor auditor)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (auditor == null)
            {
                throw new ArgumentNullException("auditor");
            }
            List<object> dataList = new List<object> { data.School.Name, data.Grade, data.FullName };
            IViewStudentDetailPermission permission = (IViewStudentDetailPermission)PermissionFactory.Current.Create("StudentProfileExportMapData", data);
            if(permission.TryGrantAccess(user))
            {
                if (!permission.CustomFieldOnly)
                {
                    dataList.Add(data.StudentSISId);
                }
                MapStandardData(fieldConfiguration, data, dataList, permission.CustomFieldOnly);
                dataList.AddRange(fieldConfiguration.SelectedCustomFields.Select(f => FindLatestValue(data, f)).Select(v => v == null ? string.Empty : v.Value));
                var fields = fieldConfiguration.SelectedCustomFields.Select(f => FindLatestValue(data, f));
                if (fields.Where(f => f != null).ToList().Count() > 0)
                {
                    user.Identity.User.PrivateHealthDataViewEvents.Add(auditor.CreatePrivateHealthInfoViewEvent(user.Identity.User, fields.Where(f => f != null).ToList()));
                }
                if (!permission.CustomFieldOnly)
                {
                    dataList.AddRange(fieldConfiguration.SelectedServiceTypes.Select(serviceType => FindServicesOfferingNames(data, serviceType)));
                }
            }
            return dataList;
        }

        IEnumerable<object> IExportDataMapper.MapData(object descriptor, object data, EducationSecurityPrincipal user, IUserAuditor auditor)
        {
            return MapData((StudentProfileExportFieldDescriptor)descriptor, (Student)data, user, auditor);
        }

        private static void MapStandardFieldHeadings(StudentProfileExportFieldDescriptor fieldConfiguration, List<string> columnHeadings)
        {
            if (fieldConfiguration.BirthDateIncluded)
            {
                columnHeadings.Add("Birth Date");
            }
            if (fieldConfiguration.ParentNameIncluded)
            {
                columnHeadings.Add("Parent Name");
            }
        }

        private static void MapStandardData(StudentProfileExportFieldDescriptor fieldConfiguration, Student data, List<object> dataList, bool directoryLevelOnly)
        {
            if (fieldConfiguration.BirthDateIncluded)
            {
                dataList.Add(directoryLevelOnly ? null : data.DateOfBirth);
            }
            if (fieldConfiguration.ParentNameIncluded)
            {
                dataList.Add(directoryLevelOnly ? null : data.Parents);
            }
        }

        private static CustomFieldValue FindLatestValue(Student data, CustomField field)
        {
            return data.CustomFieldValues.Where(v => field == v.CustomField).OrderByDescending(v => v.CustomDataOrigin.CreateTime).FirstOrDefault();
        }

        private static List<string> FindServicesOfferingNames(Student data, ServiceType serviceType)
        {
            return data.StudentAssignedOfferings.
                Where(s => s.ServiceOffering.ServiceType == serviceType && s.IsActive).
                Select(o => o.ServiceOffering.Name).ToList();
        }
    }
}
