using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Business;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ManageAssignedOfferingPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullAssignedOffering_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ManageAssignedOfferingPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(new StudentAssignedOffering());

            target.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenAssignedOfferingIsInactive_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(new StudentAssignedOffering { IsActive = false });
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.ExpectException<EntityNotFoundException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(new StudentAssignedOffering { IsActive = true });
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssociatedWithAllSchools_WhenGrantAccess_ThenSucceed()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssociatedWithNoSchools_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssociatedWithDifferentSchoolsThanStudentInAssignedOffering_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools.Where(s => s.Id != Data.StudentAssignedOfferings.First().Student.SchoolId).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssociatedWithAllSchools_AndServiceTypeForAssignedOfferingIsPrivate_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);
            Data.StudentAssignedOfferings[0].ServiceOffering.ServiceType.IsPrivate = true;

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsAssociatedWithNoSchools_AndUserCreatedAssignedOffering_WhenGrantAccess_ThenSucceed()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());
            user.Identity.User.Id = 1;
            Data.StudentAssignedOfferings[0].CreatingUserId = 1;

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithNoProviders_AndUserCreatedAssignedOffering_WhenGrantAccess_ThenSucceed()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());
            user.Identity.User.Id = 1;
            Data.StudentAssignedOfferings[0].CreatingUserId = 1;

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithAllProviders_AndAssignedOfferingStudentApprovedUserProvider_WhenGrantAccess_ThenSucceed()
        {
            var offering = Data.StudentAssignedOfferings[0];
            offering.Student.StudentAssignedOfferings = new List<StudentAssignedOffering> { offering };
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(offering);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithAllProviders_AndAssignedOfferingStudentApprovedNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);
            Data.StudentAssignedOfferings[0].Student.ApprovedProviders.Clear();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithDifferentProvidersThansAssignedOfferingStudentApproved_WhenGrantAccess_ThenThrowException()
        {
            Data.StudentAssignedOfferings[0].Student.ApprovedProviders = Data.Providers.Where(p => p != Data.StudentAssignedOfferings[0].ServiceOffering.Provider).Take(1).ToList();
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => !Data.StudentAssignedOfferings[0].Student.ApprovedProviders.Contains(p)).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithDifferentProvidersThanAssignedOffering_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p.Id != Data.StudentAssignedOfferings[0].ServiceOffering.ProviderId).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserIsAssociatedWithAllProviders_AndAssignedOfferingServiceTypeIsPrivate_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);
            Data.StudentAssignedOfferings[0].ServiceOffering.ServiceType.IsPrivate = true;

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(new StudentAssignedOffering { IsActive = true });
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            ManageAssignedOfferingPermission target = new ManageAssignedOfferingPermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
