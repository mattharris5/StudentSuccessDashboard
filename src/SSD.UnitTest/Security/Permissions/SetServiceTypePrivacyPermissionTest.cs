using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class SetServiceTypePrivacyPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenArgumentNullExceptionThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new SetServiceTypePrivacyPermission().GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenGrantAccess_ThenEntityAccessUnauthorizedExceptionThrown()
        {
            SetServiceTypePrivacyPermission target = new SetServiceTypePrivacyPermission();
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            SetServiceTypePrivacyPermission target = new SetServiceTypePrivacyPermission();
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            SetServiceTypePrivacyPermission target = new SetServiceTypePrivacyPermission();
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
