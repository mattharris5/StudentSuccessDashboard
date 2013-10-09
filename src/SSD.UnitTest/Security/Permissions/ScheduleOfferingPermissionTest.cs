using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ScheduleOfferingPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullStudentList_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ScheduleOfferingPermission(null, new ServiceOffering()));
        }

        [TestMethod]
        public void GivenEmptyStudentList_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentException>(() => new ScheduleOfferingPermission(Enumerable.Empty<Student>(), new ServiceOffering()));
        }

        [TestMethod]
        public void GivenNullServiceOffering_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentException>(() => new ScheduleOfferingPermission(Data.Students, null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);

            target.ExpectException<ArgumentException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndOfferingNotPassedIn_WhenGrantAccess_ThenSucceed()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProviderAssociatedToOffering_WhenGrantAccess_ThenSucceed()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider> { Data.ServiceOfferings[0].Provider });

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProviderNotAssociatedToOffering_WhenGrantAccess_ThenSucceed()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssignedNoSchools_WhenGrantAccess_ThenThrowException()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssignedAllSchools_WhenGrantAccess_ThenSucceed()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserNotAssignedSchoolsForAllStudents_WhenGrantAccess_ThenThrowException()
        {
            ScheduleOfferingPermission target = new ScheduleOfferingPermission(Data.Students, Data.ServiceOfferings[0]);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools.Where(s => s != Data.Students.First().School).ToList());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
