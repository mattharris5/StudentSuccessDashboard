using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ViewStudentDetailPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullStudent_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ViewStudentDetailPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student());

            TestExtensions.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndSiteCoordinatorAssignedNoSchools_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndSiteCoordinatorAssignedAllSchools_WhenGrantAccess_ThenSucceed()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { School = Data.Schools.First() });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndSiteCoordinatorAssignedNoSchools_AndStudentOptedOut_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { School = Data.Schools.First(), HasParentalOptOut = true });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndSiteCoordinatorAssignedAllSchools_AndStudentOptedOut_WhenGrantAccess_ThenSucceed()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { School = Data.Schools.First(), HasParentalOptOut = true });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndSiteCoordinatorAssignedSchoolsDifferentThanStudent_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { School = Data.Schools.First() });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools.Skip(1).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndStudentApprovedNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student());
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentIsOptedOut_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers, HasParentalOptOut = true });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentIsOptedOut_AndStudentHasActiveServiceAssignmentWithProvider_WhenGrantAccess_ThenSucceed()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers, StudentAssignedOfferings = new List<StudentAssignedOffering> { new StudentAssignedOffering { ServiceOffering = new ServiceOffering { Provider = Data.Providers[0] }, IsActive = true } }, HasParentalOptOut = true });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedProvidersDifferentThanStudentApproves_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers.Take(1).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Skip(1).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentAssignedNoOfferings_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentApprovedAllProviders_AndStudentAssignedOffering_WhenGrantAccess_ThenSucceed()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers, StudentAssignedOfferings = Data.StudentAssignedOfferings.Take(1).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProvidersAndStudentApprovedAllProviders_AndStudentAssignedInactiveOfferings_WhenGrantAccess_ThenThrowException()
        {
            Data.StudentAssignedOfferings[0].IsActive = false;
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers, StudentAssignedOfferings = Data.StudentAssignedOfferings.Take(1).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedDifferentProvidersThanAssignedOffering_AndStudentApprovedAllProviders_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers, StudentAssignedOfferings = Data.StudentAssignedOfferings.Take(1).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.StudentAssignedOfferings.First().ServiceOffering.Provider).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedProvidersDifferentThanApprovedByStudent_AndStudentAssignedOfferingAssociatedWithUser_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { ApprovedProviders = Data.Providers.Take(1).ToList(), StudentAssignedOfferings = Data.StudentAssignedOfferings.Where(s => s.ServiceOffering.Provider != Data.Providers.First()).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Skip(1).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_AndStudentAssignedOffering_AndStudentApprovesNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { StudentAssignedOfferings = Data.StudentAssignedOfferings.Take(1).ToList() });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.StudentAssignedOfferings.First().ServiceOffering.Provider).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserApprovedNoProviders_AndUserIdUploadedCustomDataToStudent_WhenGrantAccess_ThenCustomFieldOnlyTrue()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { CustomFieldValues = Data.CustomFieldValues });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.StudentAssignedOfferings.First().ServiceOffering.Provider).ToList());
            user.Identity.User.Id = Data.CustomFieldValues.First().CustomDataOrigin.CreatingUser.Id;

            target.GrantAccess(user);

            Assert.IsTrue(target.CustomFieldOnly);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserApprovedNoProviders_AndStudentHasCustomDataNotAssociatedWithUser_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { CustomFieldValues = Data.CustomFieldValues });
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.StudentAssignedOfferings.First().ServiceOffering.Provider).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenGrantAccessInvokedOnCustomFieldOnlyUser_AndCalledAgainOnDataAdmin_WhenGrantAccess_ThenCustomFieldOnlyFalse()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(new Student { CustomFieldValues = Data.CustomFieldValues });
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(Data.CustomFieldValues.First().CustomDataOrigin.CreatingUser);
            Data.CustomFieldValues.First().CustomDataOrigin.CreatingUser.UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = new Role
                    {
                        Name = SecurityRoles.Provider
                    }
                }
            };
            target.GrantAccess(user);
            user = CreateDataAdminUser();

            target.GrantAccess(user);

            Assert.IsFalse(target.CustomFieldOnly);
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            ViewStudentDetailPermission target = new ViewStudentDetailPermission(Data.Students[0]);
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
