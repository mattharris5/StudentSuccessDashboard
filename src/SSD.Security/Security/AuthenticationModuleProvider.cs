using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Services;
using System.IdentityModel.Services.Configuration;
using System.IdentityModel.Tokens;

namespace SSD.Security
{
    public class AuthenticationModuleProvider : IAuthenticationModuleProvider
    {
        private static bool _IsInitialized = false;
        private static object _LockObject = new object();

        public static Action SignedInCallback { get; private set; }

        public static void Initialize(Action signedInCallback)
        {
            lock (_LockObject)
            {
                if (!_IsInitialized)
                {
                    // NOTE: This handler is intended to solve the issue outlined here: http://blogs.microsoft.co.il/blogs/applisec/archive/2012/07/23/running-wif-relying-parties-in-windows-azure.aspx
                    FederatedAuthentication.FederationConfigurationCreated += OnFederationConfigurationCreated;
                    _IsInitialized = true;
                }
                SignedInCallback = signedInCallback;
            }
        }

        public WSFederationAuthenticationModule GetModule()
        {
            if (!_IsInitialized)
            {
                throw new InvalidOperationException(string.Format("A call to {0}.Initialize is required at application startup.", GetType().Name));
            }
            return FederatedAuthentication.WSFederationAuthenticationModule;
        }

        private static void OnFederationConfigurationCreated(object sender, FederationConfigurationCreatedEventArgs e)
        {
            List<CookieTransform> sessionTransforms = new List<CookieTransform>(new CookieTransform[]
            {
                new DeflateCookieTransform(),
                new RsaEncryptionCookieTransform(e.FederationConfiguration.ServiceCertificate),
                new RsaSignatureCookieTransform(e.FederationConfiguration.ServiceCertificate)
            });
            SessionSecurityTokenHandler sessionHandler = new SessionSecurityTokenHandler(sessionTransforms.AsReadOnly());
            e.FederationConfiguration.IdentityConfiguration.SecurityTokenHandlers.AddOrReplace(sessionHandler);
        }
    }
}
