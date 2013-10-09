using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class CustomFieldDataPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullCustomField_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new CustomFieldDataPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField());

            target.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndCustomFieldIsPublicField_WhenGrantAccess_ThenSucceed()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PublicField());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndCustomFieldIsPrivateFieldWithNoProvider_WhenGrantAccess_ThenThrowException()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndCustomFieldIsPrivateFieldWithProvider_WhenGrantAccess_ThenThrowException()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField { Provider = Data.Providers[0], ProviderId = Data.Providers[0].Id });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndCustomFieldIsProviderFieldWithAssociatedProvider_WhenGrantAccess_ThenSucceed()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField { Provider = Data.Providers[0], ProviderId = Data.Providers[0].Id });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndCustomFieldIsProviderFieldNotWithAssociatedProvider_WhenGrantAccess_ThenSucceed()
        {
            CustomFieldDataPermission target = new CustomFieldDataPermission(new PrivateHealthField());
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
