using System.IdentityModel.Services;

namespace SSD.Security
{
    public interface IAuthenticationModuleProvider
    {
        WSFederationAuthenticationModule GetModule();
    }
}
