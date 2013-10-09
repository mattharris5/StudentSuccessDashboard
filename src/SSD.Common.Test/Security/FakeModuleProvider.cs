using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;

namespace SSD.Security
{
    public class FakeModuleProvider : IAuthenticationModuleProvider
    {
        public static readonly WSFederationAuthenticationModule Singleton = new WSFederationAuthenticationModule();

        public WSFederationAuthenticationModule ModuleToReturn { get; set; }

        public WSFederationAuthenticationModule GetModule()
        {
            var instance = ModuleToReturn ?? Singleton;
            if (instance.FederationConfiguration == null)
            {
                instance.FederationConfiguration = new FederationConfiguration(false);
            }
            return instance;
        }
    }
}
