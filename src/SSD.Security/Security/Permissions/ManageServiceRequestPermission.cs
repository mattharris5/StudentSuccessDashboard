using SSD.Domain;
using System;
using System.Linq;

namespace SSD.Security.Permissions
{
    public class ManageServiceRequestPermission : BasePermission
    {
        public ManageServiceRequestPermission(ServiceRequest serviceRequest)
        {
            if (serviceRequest == null)
            {
                throw new ArgumentNullException("serviceRequest");
            }
            ServiceRequest = serviceRequest;
        }

        private ServiceRequest ServiceRequest { get; set; }

        public override void GrantAccess(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.Identity.User.UserRoles.Any())
            {
                if (IsDataAdmin(user)
                    || IsCreatingUser(user, ServiceRequest)
                    || (!ServiceRequest.ServiceType.IsPrivate &&
                        (IsSiteCoordinatorAssociatedToSchools(user, new[] { ServiceRequest.Student.School })
                        || IsApprovedProviderAssociatedToStudentOfferings(user, ServiceRequest.Student)))) 
                {
                    return;
                }
            }
            throw new EntityAccessUnauthorizedException("Not authorized to view this request.");
        }
    }
}
