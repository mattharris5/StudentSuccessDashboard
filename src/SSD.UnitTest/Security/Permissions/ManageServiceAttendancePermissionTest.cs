using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ManageServiceAttendancePermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullAssignedOffering_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ManageServiceAttendancePermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(new StudentAssignedOffering());

            target.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(new StudentAssignedOffering());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssignedNoSchools_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssignedAllSchools_WhenGrantAccess_ThenSucceed()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssignedDifferentSchoolsThanAssignedOfferingStudent_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools.Where(s => s != Data.StudentAssignedOfferings.First().Student.School).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedNoProviders_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedAllProviders_WhenGrantAccess_ThenSucceed()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndUserAssignedDifferentProvidersThanAssignedServiceOffering_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateProviderUser(Data.Providers.Where(p => p != Data.StudentAssignedOfferings.First().ServiceOffering.Provider).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserHasNoRole_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings.First());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceAttendancePermission target = new ManageServiceAttendancePermission(Data.StudentAssignedOfferings[0]);
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
