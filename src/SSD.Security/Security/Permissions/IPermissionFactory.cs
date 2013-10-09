
namespace SSD.Security.Permissions
{
    public interface IPermissionFactory
    {
        IPermission Create(string activity, params object[] args);
    }
}
