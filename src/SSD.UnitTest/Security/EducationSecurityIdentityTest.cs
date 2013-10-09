using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SSD.Security
{
    [TestClass]
    public class EducationSecurityIdentityTest
    {
        private List<User> Users { get; set; }
        private IUserRepository MockUserRepository { get; set; }
        private ClaimsIdentity BaseIdentity { get; set; }
        private EducationSecurityIdentity Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Users = new List<User> { new User { Id = 20, UserKey = "3u2e2" }, new User { Id = 30, UserKey = "29e8r2fj" }, new User { Id = 40, UserKey = "w8iw2j2" } };
            MockUserRepository = MockRepository.GenerateMock<IUserRepository>();
            MockUserRepository.Expect(r => r.Items).Return(Users.AsQueryable());
            BaseIdentity = new ClaimsIdentity("Federated");
            Target = new EducationSecurityIdentity(BaseIdentity, Users[0]);
        }

        [TestMethod]
        public void GiveNullClaimsIdentity_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new EducationSecurityIdentity(null, Users[0]));
        }

        [TestMethod]
        public void GiveNullUserIdentity_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new EducationSecurityIdentity(BaseIdentity, null));
        }

        [TestMethod]
        public void GivenFederatedClaimsIdentity_WhenIGetAuthenticationType_ThenTypeIsFederated()
        {
            Assert.AreEqual(BaseIdentity.AuthenticationType, Target.AuthenticationType);
        }

        [TestMethod]
        public void GivenNullUser_WhenIGetUserEntity_ThenReturnNull()
        {
            Assert.AreEqual(Users[0], Target.User);
        }

        [TestMethod]
        public void GivenClaimsIdentityHasClaim_WhenIGetIsAuthenticated_ThenReturnTrue()
        {
            BaseIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "whatever"));
            Assert.IsTrue(Target.IsAuthenticated);
        }

        [TestMethod]
        public void GivenClaimsIdentityHasNoClaims_WhenIGetIsAuthenticated_ThenReturnTrue()
        {
            Target = new EducationSecurityIdentity(new ClaimsIdentity(), Users[0]);
            Assert.IsFalse(Target.IsAuthenticated);
        }

        [TestMethod]
        public void GivenNullClaimsIdentity_WhenIFindUserKey__ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => EducationSecurityIdentity.FindUserKey(null));
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNameIdentifierClaim_WhenIFindUserKey_ThenGetNameIdentifierClaimValue()
        {
            string expected = "this is the user key";
            BaseIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, expected));

            string actual = EducationSecurityIdentity.FindUserKey(BaseIdentity);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenClaimsIdentityWithNoNameIdentifierClaim_WhenIFindUserKey_ThenThrowException()
        {
            Target.ExpectException<InvalidOperationException>(() => EducationSecurityIdentity.FindUserKey(new ClaimsIdentity()));
        }

        [TestMethod]
        public void WhenGetIdentityName_ThenDoNotReturnNull()
        {
            Assert.IsNotNull(Target.Name);
        }

        [TestMethod]
        public void GivenUserHasDisplayName_WhenGetIdentityName_ThenReturnDisplayName()
        {
            string expected = "this is the name I expect";
            Users[0].DisplayName = expected;

            string actual = Target.Name;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenBaseIdentityHasName_AndUserHasDisplayName_WhenGetIdentityName_ThenReturnBaseIdentityNameInsteadOfUserName()
        {
            string expected = "this is the name I expect";
            string notExpected = "this is the name of the user, but defer to base identity for name";
            Users[0].DisplayName = notExpected;
            BaseIdentity = new GenericIdentity(expected, "Federated");
            Target = new EducationSecurityIdentity(BaseIdentity, Users[0]);

            string actual = Target.Name;

            Assert.AreEqual(expected, actual);
        }
    }
}
