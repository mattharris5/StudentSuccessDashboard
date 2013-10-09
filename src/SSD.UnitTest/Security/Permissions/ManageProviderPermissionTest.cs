using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ManageProviderPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_WhenGrantAccess_ThenSucceed()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedDifferentProvidersThanSpecifiedToPermission_WhenGrantAccess_ThenThrowException()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p.Id != 1).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_WhenGrantAccess_ThenSucceed()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserAssignedNoRoles_WhenGrantAccess_ThenThrowException()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleUnknown_WhenGrantAccess_ThenThrowException()
        {
            ManageProviderPermission target = new ManageProviderPermission(1);
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
