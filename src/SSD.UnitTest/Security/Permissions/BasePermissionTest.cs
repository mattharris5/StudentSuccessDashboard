using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System.Collections.Generic;

namespace SSD.Security.Permissions
{
    [TestClass]
    public abstract class BasePermissionTest
    {
        protected TestData Data { get; private set; }

        [TestInitialize]
        public void BaseInitializeTest()
        {
            Data = new TestData();
        }

        protected EducationSecurityPrincipal CreateDataAdminUser()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User
            {
                UserKey = "whatever",
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            Name = SecurityRoles.DataAdmin
                        }
                    }
                }
            });
            return user;
        }

        protected EducationSecurityPrincipal CreateSiteCoordinatorUser(ICollection<School> associatedSchools)
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User
            {
                UserKey = "whatever",
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            Name = SecurityRoles.SiteCoordinator
                        },
                        Schools = associatedSchools
                    }
                }
            });
            return user;
        }

        protected EducationSecurityPrincipal CreateProviderUser(ICollection<Provider> associatedProviders)
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User
            {
                UserKey = "whatever",
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            Name = SecurityRoles.Provider
                        },
                        Providers = associatedProviders
                    }
                }
            });
            return user;
        }

        protected EducationSecurityPrincipal CreateUserWithNoRoles()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User
            {
                UserKey = "whatever"
            });
            return user;
        }

        protected EducationSecurityPrincipal CreateUserWithUnknownRole()
        {
            EducationSecurityPrincipal user = new EducationSecurityPrincipal(new User
            {
                UserKey = "whatever",
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role
                        {
                            Name = "Unknown"
                        }
                    }
                }
            });
            return user;
        }
    }
}
