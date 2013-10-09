using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class CreateServiceRequestPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullStudentIds_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new CreateServiceRequestPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(Enumerable.Empty<Student>());

            TestExtensions.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(Enumerable.Empty<Student>());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(Enumerable.Empty<Student>());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserAssociatedWithSchool_WhenGrantAccess_ThenSucceed()
        {
            var entityList = new List<Student>
            {
                new Student
                {
                    School = new School()
                }
            };
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(entityList);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(entityList.Select(s => s.School).ToList());

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndDoesntHaveAccessToAll_ThenThrowException()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(new List<Student>
            {
                new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider>()
                }
            });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsProvider_WhenGrantAccess_ThenThrowException()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(new List<Student>());
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            CreateServiceRequestPermission target = new CreateServiceRequestPermission(new List<Student>());
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
