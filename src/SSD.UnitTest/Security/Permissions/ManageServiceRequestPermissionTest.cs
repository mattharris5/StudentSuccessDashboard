using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;

namespace SSD.Security.Permissions
{
    [TestClass]
    public class ManageServiceRequestPermissionTest : BasePermissionTest
    {
        [TestMethod]
        public void GivenNullServiceRequest_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ManageServiceRequestPermission(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest());

            TestExtensions.ExpectException<ArgumentNullException>(() => target.GrantAccess(null));
        }

        [TestMethod]
        public void GivenUserHasNoRoles_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest());
            EducationSecurityPrincipal user = CreateUserWithNoRoles();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGrantAccess_ThenSucceed()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                ServiceType = new ServiceType(),
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider> ()
                }
            });
            EducationSecurityPrincipal user = CreateDataAdminUser();

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndCreatedRequest_ThenSucceed()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                ServiceType = new ServiceType(),
                CreatingUserId = 1,
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider>()
                }
            });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());
            user.Identity.User.Id = 1;

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndDidntCreateRequest_AndServiceTypeIsntPrivate_AndUserAssociatedWithSchool_WhenGrantAccess_ThenSucceed()
        {
            var entity = new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole> { new UserRole { UserId = 1 } } }
                }
            };
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(entity);
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new[] { entity.Student.School });

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsProvider_AndDidntCreateRequest_AndServiceTypeIsntPrivate_AndUserAssociatedWithApprovedProviders_AndProvidersAssociatedWithStudentAssignedOfferings_WhenGrantAccess_ThenSucceed()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider> { Data.Providers[0] },
                    StudentAssignedOfferings = new List<StudentAssignedOffering>
                    {
                        new StudentAssignedOffering()
                        {
                            IsActive = true,
                            ServiceOffering = new ServiceOffering{ Provider = Data.Providers[0] }
                        }
                    }
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider> { Data.Providers[0] });

            target.GrantAccess(user);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDidntCreateRequest_AndServiceTypeIsntPrivate_AndUserAssociatedWithApprovedProviders_AndProvidersAssociatedWithStudentAssignedOfferingsThatArentActive_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider> { Data.Providers[0] },
                    StudentAssignedOfferings = new List<StudentAssignedOffering>
                    {
                        new StudentAssignedOffering()
                        {
                            IsActive = false,
                            ServiceOffering = new ServiceOffering{ Provider = Data.Providers[0] }
                        }
                    }
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider> { Data.Providers[0] });

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDidntCreateRequest_AndServiceTypeIsntPrivate_AndUserAssociatedWithApprovedProviders_AndProvidersNotAssociatedWithStudentAssignedOfferings_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider> { Data.Providers[0] },
                    StudentAssignedOfferings = new List<StudentAssignedOffering>
                    {
                        new StudentAssignedOffering()
                        {
                            ServiceOffering = new ServiceOffering{ Provider = new Provider() }
                        }
                    }
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider> { Data.Providers[0] });

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDidntCreateRequest_AndServiceTypeIsPrivate_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 1,
                ServiceType = new ServiceType { IsPrivate = true },
                Student = new Student()
            });
            EducationSecurityPrincipal user = CreateSiteCoordinatorUser(new List<School>());
            user.Identity.User.Id = 2;

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_AndDidntCreateRequest_AndServiceTypeIsntPrivate_AndUserNotAssociatedToSchools_AndUserNotAssociatedToProviders_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider>()
                }
            });
            EducationSecurityPrincipal user = CreateProviderUser(new List<Provider>());

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }

        [TestMethod]
        public void GivenUserRoleIsUnknown_WhenGrantAccess_ThenThrowException()
        {
            ManageServiceRequestPermission target = new ManageServiceRequestPermission(new ServiceRequest
            {
                CreatingUserId = 2,
                ServiceType = new ServiceType
                {
                    IsPrivate = false
                },
                Student = new Student
                {
                    School = new School { UserRoles = new List<UserRole>() },
                    ApprovedProviders = new List<Provider>()
                }
            });
            EducationSecurityPrincipal user = CreateUserWithUnknownRole();

            target.ExpectException<EntityAccessUnauthorizedException>(() => target.GrantAccess(user));
        }
    }
}
