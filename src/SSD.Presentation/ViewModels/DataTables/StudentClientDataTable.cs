using SSD.Domain;
using SSD.Security;
using SSD.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    public class StudentClientDataTable : BaseClientDataTable<Student>
    {
        public StudentClientDataTable(HttpRequestBase request, EducationSecurityPrincipal currentUser, IEnumerable<Property> studentProperties)
            : base(request)
        {
            InitializeRequestValues(request);
            IsAdministrator = currentUser.IsInRole(SecurityRoles.DataAdmin);
            IsSiteCoordinator = currentUser.IsInRole(SecurityRoles.SiteCoordinator);
            IsProvider = currentUser.IsInRole(SecurityRoles.Provider);
            AssociatedSchoolIds = currentUser.Identity.User.UserRoles.SelectMany(ur => ur.Schools).Select(s => s.Id).ToList();
            AssociatedProviderIds = currentUser.Identity.User.UserRoles.SelectMany(ur => ur.Providers).Select(p => p.Id).ToList();
            CurrentUserId = currentUser.Identity.User.Id;
            int assignedServicePropertyId = GetPropertyId(studentProperties, () => (new Student()).StudentAssignedOfferings);
            int serviceRequestPropertyId = GetPropertyId(studentProperties, () => (new Student()).ServiceRequests);
            IsAssignedServiceProtected = studentProperties.Where(p => p.Id == assignedServicePropertyId).Select(a => a.IsProtected).Distinct().Contains(true);
            IsServiceRequestProtected = studentProperties.Where(p => p.Id == serviceRequestPropertyId).Select(p => p.IsProtected).Distinct().Contains(true);
            User = currentUser;
        }

        public string Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public IEnumerable<string> Schools { get; private set; }
        public IEnumerable<int> Grades { get; private set; }
        public IEnumerable<string> Priorities { get; private set; }
        public IEnumerable<string> RequestStatuses { get; private set; }
        public IEnumerable<string> ServiceTypes { get; private set; }
        public IEnumerable<string> Subjects { get; private set; }
        public bool IsAdministrator { get; private set; }
        public bool IsSiteCoordinator { get; private set; }
        public bool IsProvider { get; private set; }
        public IEnumerable<int> AssociatedSchoolIds { get; private set; }
        public IEnumerable<int> AssociatedProviderIds { get; private set; }
        public int CurrentUserId { get; private set; }
        private bool IsAssignedServiceProtected { get; set; }
        private bool IsServiceRequestProtected { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        public override Expression<Func<Student, string>> SortSelector
        {
            get
            {
                if (SortColumnIndex == 2)
                {
                    return s => s.FullName;
                }
                if (SortColumnIndex == 3)
                {
                    return s => SqlFunctions.StringConvert((double)s.Grade);
                }
                if (SortColumnIndex == 4)
                {
                    return s => s.School.Name;
                }
                return s => s.StudentSISId;
            }
        }

        public override Expression<Func<Student, bool>> FilterPredicate
        {
            get
            {
                Expression<Func<Student, bool>> filterPredicate = s => !(!IsAdministrator && !AssociatedSchoolIds.Contains(s.SchoolId) && !AssociatedProviderIds.Intersect(s.ApprovedProviders.Select(ap => ap.Id)).Any() && s.HasParentalOptOut);
                if (Id != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.StudentSISId.ToLower().Contains(Id.ToLower()));
                }
                if (FirstName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.FirstName.ToLower().Contains(FirstName.ToLower()));
                }
                if (LastName != null)
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.LastName.ToLower().Contains(LastName.ToLower()));
                }
                if (Schools.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => Schools.Contains(s.School.Name));
                }
                if (Grades.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => Grades.Contains(s.Grade));
                }
                if (Priorities.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.ServiceRequests.Any(request => Priorities.Contains(request.Priority.Name)));
                }
                if (RequestStatuses.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.ServiceRequests.Any(request => RequestStatuses.Contains(request.FulfillmentDetails.OrderByDescending(f => f.CreateTime).Select(f => f.FulfillmentStatus.Name).FirstOrDefault())));
                }
                if (ServiceTypes.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.ServiceRequests.Any(request => ServiceTypes.Contains(request.ServiceType.Name)));
                }
                if (Subjects.Any())
                {
                    filterPredicate = filterPredicate.AndAlso(s => s.ServiceRequests.Any(request => Subjects.Contains(request.Subject.Name)));
                }
                return filterPredicate;
            }
        }

        public override Expression<Func<Student, object>> DataSelector
        {
            get
            {
                return s => new[]
                {
                    CreateServiceRequestString(s),
                    CreateStudentSISIdString(s),
                    CreateNameString(s),
                    s.Grade.ToString(CultureInfo.CurrentCulture),
                    s.School.Name,
                    CreateServiceOfferingString(s),
                    CreateCheckbox(s),
                };
            }
        }

        private static int GetPropertyId<T>(IEnumerable<Property> studentProperties, Expression<Func<T>> expression)
        {
            string entityName = ((MemberExpression)expression.Body).Member.ReflectedType.FullName;
            string propertyName = PropertyReflection.GetMemberName<T>(expression);
            return studentProperties.Where(p => p.EntityName.Equals(entityName) && p.Name.Equals(propertyName)).Select(p => p.Id).SingleOrDefault();
        }

        private void InitializeRequestValues(HttpRequestBase request)
        {
            Id = ExtractFilterValue("ID");
            FirstName = ExtractFilterValue("firstName");
            LastName = ExtractFilterValue("lastName");
            Schools = ExtractFilterList("schools");
            Grades = ExtractGradeList();
            Priorities = ExtractFilterList("priorities");
            RequestStatuses = ExtractFilterList("requestStatuses");
            ServiceTypes = ExtractFilterList("serviceTypes");
            Subjects = ExtractFilterList("subjects");
        }

        private bool ShowAssignedServices(Student student)
        {
            return !IsDirectoryLevel(student) || !IsAssignedServiceProtected;
        }

        private bool ShowServiceRequests(Student student)
        {
            return !IsDirectoryLevel(student) || !IsServiceRequestProtected;
        }

        private bool IsDirectoryLevel(Student student)
        {
            var hasRightsToSchool = AssociatedSchoolIds.Contains(student.SchoolId);
            var hasProviderAccessRights = AssociatedProviderIds.Intersect(student.ApprovedProviders.Select(ap => ap.Id)).Any();
            return !IsAdministrator && !hasRightsToSchool && !hasProviderAccessRights;
        }

        private string CreateCheckbox(Student student)
        {
            var hasRightsToSchool = AssociatedSchoolIds.Contains(student.SchoolId);
            if (IsAdministrator || hasRightsToSchool || IsProvider)
            {
                return student.Id.ToString(CultureInfo.CurrentCulture);
            }
            return string.Empty;
        }

        private string CreateServiceRequestString(Student student)
        {
            var priorities = new List<string>();
            foreach (ServiceRequest request in student.ServiceRequests)
            {
                IPermission permission = PermissionFactory.Current.Create("CreateServiceRequestString", request);
                if (permission.TryGrantAccess(User))
                {
                    if (IsAdministrator || IsSiteCoordinator)
                    {
                        priorities.Add(CreateServiceRequestString(request, true));
                    }
                    else if (IsProvider)
                    {
                        priorities.Add(CreateServiceRequestString(request, false));
                    }
                }
            }
            if (priorities.Count > 0)
            {
                return BuildListString(priorities);
            }
            else
            {
                return string.Empty;
            }
        }

        private static string CreateServiceRequestString(ServiceRequest request, bool allowEdit)
        {
            string allowEditIndicator = allowEdit ? "Y" : "N";
            string subject = request.Subject.Name.Equals(Subject.DefaultName) ? "" : request.Subject.Name;
            string status;
            if (request.FulfillmentDetails != null && request.FulfillmentDetails.Count() > 0)
            {
                status = request.FulfillmentDetails.OrderByDescending(f => f.CreateTime).First().FulfillmentStatusId.ToString();
            }
            else
            {
                status = "1";
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}|{1}|{2}|{3}|{4}|{5}|", allowEditIndicator, request.Id, request.ServiceType.Name, request.Priority.Id, subject, status);
        }

        private string CreateStudentSISIdString(Student student)
        {
            IViewStudentDetailPermission permission = (IViewStudentDetailPermission)PermissionFactory.Current.Create("ViewStudentDetail", student);
            return permission.TryGrantAccess(User) && !permission.CustomFieldOnly ? student.StudentSISId : "";
        }

        private string CreateNameString(Student student)
        {
            IViewStudentDetailPermission permission = (IViewStudentDetailPermission)PermissionFactory.Current.Create("CreateStudentNameString", student);
            if(permission.TryGrantAccess(User))
            {
                return string.Format(CultureInfo.CurrentCulture, "{0}|{1}", student.FullName, student.Id);
            }
            return string.Format(CultureInfo.CurrentCulture, "{0}", student.FullName);
        }

        private string CreateServiceOfferingString(Student student)
        {
            if (ShowAssignedServices(student) || student.StudentAssignedOfferings.Select(s => s.ServiceOffering.ProviderId).Intersect(AssociatedProviderIds).Any())
            {
                var serviceOfferings = new List<string>();
                foreach (StudentAssignedOffering offering in student.StudentAssignedOfferings.Where(s => s.IsActive))
                {
                    if (IsAdministrator || IsSiteCoordinator || AssociatedProviderIds.Contains(offering.ServiceOffering.ProviderId))
                    {
                        serviceOfferings.Add(CreateServiceOfferingString(offering, true));
                    }
                    else if (!offering.ServiceOffering.ServiceType.IsPrivate && IsProvider)
                    {
                        serviceOfferings.Add(CreateServiceOfferingString(offering, false));
                    }
                }
                if (serviceOfferings.Count > 0)
                {
                    return BuildListString(serviceOfferings);
                }
            }
            return string.Empty;
        }

        private static string CreateServiceOfferingString(StudentAssignedOffering offering, bool allowEdit)
        {
            string allowEditIndicator = allowEdit ? "Y" : "N";
            return string.Format(CultureInfo.CurrentCulture, "{0}|{1}|{2}|", allowEditIndicator, offering.Id, offering.ServiceOffering.Name);
        }

        private static string BuildListString(IEnumerable<string> listItems)
        {
            var returnResult = string.Concat(listItems);
            return returnResult.Remove(returnResult.Length - 1);
        }

        private List<int> ExtractGradeList()
        {
            var grades = ExtractFilterList("grades");
            if (grades == null)
            {
                return new List<int>();
            }
            List<int> finalGrades = new List<int>();
            foreach (string g in grades)
            {
                int gradeInt;
                if (int.TryParse(g, out gradeInt))
                {
                    finalGrades.Add(gradeInt);
                }
            }
            return finalGrades;
        }
    }
}
