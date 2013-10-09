using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Security
{
    [TestClass]
    public class AuthenticationModuleTest
    {
        [TestMethod]
        public void GivenAuthenticationModuleProviderInitializedWithCallback_WhenOnSignedIn_ThenCallbackInvoked()
        {
            bool wasCalled = false;
            AuthenticationModuleProvider.Initialize(new Action(() => wasCalled = true));
            TestTarget target = new TestTarget();

            target.CallOnSignedIn();

            Assert.IsTrue(wasCalled);
        }

        private class TestTarget : AuthenticationModule
        {
            public void CallOnSignedIn()
            {
                OnSignedIn(null);
            }
        }
    }
}
