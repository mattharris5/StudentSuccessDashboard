using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ManageCustomFieldPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullStudent_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ManageCustomFieldPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student());

            TestExtensions.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student());
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndNotAssociatedToStudentSchool_WhenGrantAccess_ThenThrowException()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student { School = Data.Schools[0] });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndAssociatedToStudentSchool_WhenGrantAccess_ThenSucceed()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student { School = Data.Schools[0] });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(Data.Schools);

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndAssociatedToServiceOffering_WhenGrantAccess_ThenSucceed()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student
            {
                School = new School(),
                StudentAssignedOfferings = new List<StudentAssignedOffering>
                {
                    new StudentAssignedOffering
                    {
                        ServiceOffering = new ServiceOffering { Provider = Data.Providers[0] }, IsActive = true
                    }
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider> { Data.Providers[0] });

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndNotAssociatedToServiceOffering_WhenGrantAccess_ThenThrowException()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(new Student
            {
                School = new School(),
                StudentAssignedOfferings = new List<StudentAssignedOffering>
                {
                    new StudentAssignedOffering
                    {
                        ServiceOffering = new ServiceOffering { Provider = Data.Providers[0] }
                    }
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            ManageCustomFieldPermission target = new ManageCustomFieldPermission(Data.Students[0]);
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
