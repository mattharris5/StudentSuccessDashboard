
namespace SSD.Security.Permissions
{
    public interface IPermission
    {
        void GrantAccess(EducationSecurityPrincipal user);
    }
}
