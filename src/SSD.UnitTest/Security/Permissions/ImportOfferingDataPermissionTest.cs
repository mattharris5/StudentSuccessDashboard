using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ImportOfferingDataPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullOffering_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ImportOfferingDataPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(new ServiceOffering());

            target.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(new ServiceOffering());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_WhenGrantAccess_ThenSucceed()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(new ServiceOffering());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserHasNoProviderAssociations_WhenGrantAccess_ThenThrowException()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(new ServiceOffering());
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserHasAllProviderAssociations_WhenGrantAccess_ThenSucceed()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserHasProviderAssociationsDifferentThanOffering_WhenGrantAccess_ThenSucceed()
        {
            ImportOfferingDataPermission target = new ImportOfferingDataPermission(Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.ServiceOfferings[0].Provider).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
