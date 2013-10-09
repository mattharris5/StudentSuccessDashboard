using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;

namespace SSD.Security
{
    [TestClass]
    public class EducationSecurityPrincipalTest
    {
        [TestMethod]
        public void GiveNullIdentity_WhenIConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new EducationSecurityPrincipal(null as EducationSecurityIdentity));
        }

        [TestMethod]
        public void GiveNullUserEntity_WhenIConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new EducationSecurityPrincipal(null as User));
        }

        [TestMethod]
        public void GivenIdentity_WhenIGetIdentity_ThenIdentityMatchesOriginal()
        {
            EducationSecurityIdentity expected = new EducationSecurityIdentity(new ClaimsIdentity(), new User());
            EducationSecurityPrincipal target = new EducationSecurityPrincipal(expected);
            Assert.AreEqual(expected, target.Identity);
            Assert.AreEqual(expected, ((IPrincipal)target).Identity);
        }

        [TestMethod]
        public void GivenUserEntity_WhenIGetIdentity_ThenIdentityUserEntityMatches()
        {
            User expected = new User { UserKey = "2r2j289fj" };
            EducationSecurityPrincipal target = new EducationSecurityPrincipal(expected);
            Assert.AreEqual(expected, target.Identity.User);
        }

        [TestMethod]
        public void GivenUserEntityWithNullUserKey_WhenIConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentException>(() => new EducationSecurityPrincipal(new User()));
        }

        [TestMethod]
        public void GivenUserEntityWhenConstructed_WhenIGetIdentityUserEntity_ThenIdentityHasCustomAuthenticationType()
        {
            EducationSecurityPrincipal target = new EducationSecurityPrincipal(new User { UserKey = "2r2j289fj" });
            Assert.AreEqual("Custom", target.Identity.AuthenticationType);
        }

        [TestMethod]
        public void GivenUserEntityWhenConstructed_WhenIGetIdentityUserEntity_ThenIdentityIsAuthenticated()
        {
            EducationSecurityPrincipal target = new EducationSecurityPrincipal(new User { UserKey = "2r2j289fj" });
            Assert.IsTrue(target.Identity.IsAuthenticated);
        }

        [TestMethod]
        public void GivenUserEntityWhenConstructed_AndUserContainsRole_WhenIQueryForIsInRole_ThenReturnFalse()
        {
            EducationSecurityPrincipal target = CreateTarget("Admin", null, null);

            Assert.IsTrue(target.IsInRole("Admin"));
        }

        [TestMethod]
        public void GivenUserEntityWhenConstructed_AndUserDoesNotContainRole_WhenIQueryForIsInRole_ThenReturnFalse()
        {
            EducationSecurityPrincipal target = CreateTarget("NonAdmin", null, null);

            Assert.IsFalse(target.IsInRole("Admin"));
        }

        [TestMethod]
        public void GivenUserEntityIsConfiguredAsAdministorator_WhenIQueryForIsInRole_ThenReturnTrue()
        {
            EducationSecurityPrincipal target = CreateTarget("Admin", "bob@bob.bob", "bob@bob.bob");

            Assert.IsTrue(target.IsInRole(SecurityRoles.Administrator));
        }

        [TestMethod]
        public void GivenUserEntityIsConfiguredAsAdministorator_AndCaseDoesNotMatch_WhenIQueryForIsInRole_ThenReturnTrue()
        {
            EducationSecurityPrincipal target = CreateTarget("Admin", "bob@bob.bob", "Bob@bob.bob");

            Assert.IsTrue(target.IsInRole(SecurityRoles.Administrator));
        }

        [TestMethod]
        public void GivenUserEntityIsNotConfiguredAsAdministorator_WhenIQueryForIsInRole_ThenReturnFalse()
        {
            EducationSecurityPrincipal target = CreateTarget("Admin", "bob1@bob.bob", "bob@bob.bob");

            Assert.IsFalse(target.IsInRole(SecurityRoles.Administrator));
        }

        [TestMethod]
        public void GivenNullUserEntity_WhenCheckIsAdministrator_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => EducationSecurityPrincipal.IsAdministrator(null, MockRepository.GenerateMock<ISecurityConfiguration>()));
        }

        [TestMethod]
        public void GivenNullSecurityConfiguration_WhenCheckIsAdministrator_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => EducationSecurityPrincipal.IsAdministrator(new User(), null));
        }

        [TestMethod]
        public void GivenNullClaimsPrincipal_WhenGetUserKey_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => EducationSecurityPrincipal.FindUserKey(null));
        }

        [TestMethod]
        public void GivenClaimsPrincipal_AndIdentityHasNameIdentifierClaim_WhenGetUserKey_ThenGetNameIdentifierClaimValue()
        {
            string expected = "this is the user key";
            GenericIdentity identity = new GenericIdentity("whatever");
            GenericPrincipal principal = new GenericPrincipal(identity, null);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, expected));

            string actual = EducationSecurityPrincipal.FindUserKey(principal);

            Assert.AreEqual(expected, actual);
        }

        private static EducationSecurityPrincipal CreateTarget(string roleName, string userEmailAddress, string adminEmailAddress)
        {
            User user = new User { UserKey = "2r2j289fj", EmailAddress = userEmailAddress };
            user.UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = roleName }, User = user } };
            return CreateTarget(adminEmailAddress, user);
        }

        private static EducationSecurityPrincipal CreateTarget(string adminEmailAddress, User user)
        {
            ISecurityConfiguration config = MockRepository.GenerateMock<ISecurityConfiguration>();
            config.Expect(m => m.AdministratorEmailAddresses).Return(new string[] { adminEmailAddress });
            EducationSecurityPrincipal target = new EducationSecurityPrincipal(user);
            target.Configuration = config;
            return target;
        }
    }
}
