using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SSD.Security
{
    [TestClass]
    public class DefaultSecurityConfigurationTest
    {
        private DefaultSecurityConfiguration Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            ConfigurationManager.AppSettings.Set("AdministratorEmailAddresses", "");
            Target = new DefaultSecurityConfiguration();
        }

        [TestCleanup]
        public void CleanupTest()
        {
            ConfigurationManager.AppSettings.Set("AdministratorEmailAddresses", "");
        }

        [TestMethod]
        public void GivenConfigurationFileHasNoAdministratorEmailAddressesSetting_WhenGetAdministratorEmailAddresses_ThenThrowException()
        {
            IEnumerable<string> unassignable = null;
            ConfigurationManager.AppSettings.Set("AdministratorEmailAddresses", null);

            Target.ExpectException<InvalidOperationException>(() => unassignable = Target.AdministratorEmailAddresses);
        }

        [TestMethod]
        public void GivenConfigurationFileHasNoCommas_WhenGetAdministratorEmailAddresses_ThenReturnSettingAsSingleItem()
        {
            string expected = "whatever@whatever.com";
            ConfigurationManager.AppSettings.Set("AdministratorEmailAddresses", expected);

            var actual = Target.AdministratorEmailAddresses;

            Assert.AreEqual(expected, actual.Single());
        }

        [TestMethod]
        public void GivenConfigurationFileHasCommas_WhenGetAdministratorEmailAddresses_ThenReturnSettingsDelimitedByCommas()
        {
            string[] expected = new[] { "whatever@whatever.com", "something@else.com", "another@item.com" };
            ConfigurationManager.AppSettings.Set("AdministratorEmailAddresses", string.Join(",", expected));

            var actual = Target.AdministratorEmailAddresses;

            CollectionAssert.AreEqual(expected, actual.ToList());
        }
    }
}
