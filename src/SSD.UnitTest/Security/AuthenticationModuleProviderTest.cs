using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Security
{
    [TestClass]
    public class AuthenticationModuleProviderTest
    {
        [TestMethod]
        public void GivenCallback_WhenInitialize_ThenCallbackSet()
        {
            var expected = new Action(GivenCallback_WhenInitialize_ThenCallbackSet);

            AuthenticationModuleProvider.Initialize(expected);

            Assert.AreEqual(expected, AuthenticationModuleProvider.SignedInCallback);
        }

        [TestMethod]
        public void GivenCallback_AndDifferentCallbackRegistered_WhenInitialize_ThenCallbackSet()
        {
            var expected = new Action(GivenCallback_WhenInitialize_ThenCallbackSet);
            AuthenticationModuleProvider.Initialize(new Action(() => { }));

            AuthenticationModuleProvider.Initialize(expected);

            Assert.AreEqual(expected, AuthenticationModuleProvider.SignedInCallback);
        }
    }
}
