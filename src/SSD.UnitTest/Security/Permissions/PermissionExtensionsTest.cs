using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class PermissionExtensionsTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullPermission_WhenTryGrantAccess_ThenArgumentNullException()
        {
            EducationSecurityPrincipal user = CreateUserWithNoRoles();
            IPermission permission = null;

            TestExtensions.ExpectException<ArgumentNullException>(() => permission.TryGrantAccess(user));
        }

        [TestMethod]
        public void GivenPermissionFailsGrantAccess_WhenTryGrantAccess_ThenReturnFalse()
        {
            EducationSecurityPrincipal user = CreateUserWithNoRoles();
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(user)).Throw(new EntityAccessUnauthorizedException());

            Assert.IsFalse(permission.TryGrantAccess(user));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenTryGrantAccess_ThenReturnTrue()
        {
            EducationSecurityPrincipal user = CreateUserWithNoRoles();
            IPermission permission = MockRepository.GenerateMock<IPermission>();

            Assert.IsTrue(permission.TryGrantAccess(user));
        }
    }
}
