using System;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Reflection;

namespace SSD.Security
{
    public class AuthenticationModule : WSFederationAuthenticationModule
    {
        protected override void OnSignedIn(EventArgs args)
        {
            Trace.WriteLine(string.Format("{0}.{1} invoked.", GetType().Name, MethodBase.GetCurrentMethod().Name), "Information");
            base.OnSignedIn(args);
            Action callback = AuthenticationModuleProvider.SignedInCallback;
            if (callback != null)
            {
                callback();
            }
        }
    }
}
