using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Security
{
    [TestClass]
    public class UserAuditorTest
    {
        private UserAuditor Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new UserAuditor();
        }

        [TestMethod]
        public void GivenNullUser_WhenCreateAccessChangeEvent_ThenThrowException()
        {
            User requestor = new User();

            TestExtensions.ExpectException<ArgumentNullException>(() => Target.CreateAccessChangeEvent(null, requestor));
        }

        [TestMethod]
        public void GivenNullRequestor_WhenCreateAccessChangeEvent_ThenThrowException()
        {
            User user = new User();

            TestExtensions.ExpectException<ArgumentNullException>(() => Target.CreateAccessChangeEvent(user, null));
        }

        [TestMethod]
        public void GivenUser_AndRequestor_WhenCreateAccessChangeEvent_ThenReturnInstance()
        {
            User user = new User();
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_WhenCreateAccessChangeEvent_ThenEventUserAndCreatingUserSet()
        {
            User expectedUser = new User { Id = 3828 };
            User expectedRequestor = new User { Id = 84620 };

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(expectedUser, expectedRequestor);

            Assert.AreEqual(expectedUser, actual.User);
            Assert.AreEqual(expectedUser.Id, actual.UserId);
            Assert.AreEqual(expectedRequestor, actual.CreatingUser);
            Assert.AreEqual(expectedRequestor.Id, actual.CreatingUserId);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserIsActive_WhenCreateAccessChangeEvent_ThenEventUserActiveTrue()
        {
            User user = new User { Active = true };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.IsTrue(actual.UserActive);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserIsNotActive_WhenCreateAccessChangeEvent_ThenEventUserActiveFalse()
        {
            User user = new User { Active = false };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.IsFalse(actual.UserActive);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_WhenCreateAccessChangeEvent_ThenEventCreateTimeSet()
        {
            User user = new User();
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.IsTrue(DateTime.Now.WithinTimeSpanOf(TimeSpan.FromSeconds(1), actual.CreateTime));
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserHasDataAdminRole_WhenCreateAccessChangeEvent_ThenAccessDataRolesElementHasDataAdminElement_AndElementAttributesSet()
        {
            int expectedRoleId = 100;
            string expectedRoleName = SecurityRoles.DataAdmin;
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role { Id = expectedRoleId, Name = expectedRoleName }
                    }
                }
            };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.AreEqual(expectedRoleId.ToString(), actual.AccessXml.Element("roles").Element("role").Attribute("id").Value);
            Assert.AreEqual(expectedRoleName, actual.AccessXml.Element("roles").Element("role").Attribute("name").Value);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserAssociatedToTwoProviders_WhenCreateAccessChangeEvent_ThenProvidersElementHasTwoProviderElements_AndElementAttributesSet()
        {
            int expectedFirstProviderId = 382;
            string expectedFirstProviderName = "Provider #1";
            int expectedSecondProviderId = 4563;
            string expectedSecondProviderName = "The other provider";
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role { Name = "whatever" },
                        Providers = new List<Provider>
                        {
                            new Provider { Id = expectedFirstProviderId, Name = expectedFirstProviderName },
                            new Provider { Id = expectedSecondProviderId, Name = expectedSecondProviderName }
                        }
                    }
                }
            };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.AreEqual(expectedFirstProviderId.ToString(), actual.AccessXml.Element("providers").Elements("provider").First().Attribute("id").Value);
            Assert.AreEqual(expectedFirstProviderName, actual.AccessXml.Element("providers").Elements("provider").First().Attribute("name").Value);
            Assert.AreEqual(expectedSecondProviderId.ToString(), actual.AccessXml.Element("providers").Elements("provider").Last().Attribute("id").Value);
            Assert.AreEqual(expectedSecondProviderName, actual.AccessXml.Element("providers").Elements("provider").Last().Attribute("name").Value);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserAssociatedToTwoSchools_WhenCreateAccessChangeEvent_ThenSchoolsElementHasTwoSchoolElements_AndElementAttributesSet()
        {
            int expectedFirstSchoolId = 78;
            string expectedFirstSchoolName = "First School";
            int expectedSecondSchoolId = 54;
            string expectedSecondSchoolName = "Second School";
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role { Name = "whatever" },
                        Schools = new List<School>
                        {
                            new School { Id = expectedFirstSchoolId, Name = expectedFirstSchoolName },
                            new School { Id = expectedSecondSchoolId, Name = expectedSecondSchoolName }
                        }
                    }
                }
            };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.AreEqual(expectedFirstSchoolId.ToString(), actual.AccessXml.Element("schools").Elements("school").First().Attribute("id").Value);
            Assert.AreEqual(expectedFirstSchoolName, actual.AccessXml.Element("schools").Elements("school").First().Attribute("name").Value);
            Assert.AreEqual(expectedSecondSchoolId.ToString(), actual.AccessXml.Element("schools").Elements("school").Last().Attribute("id").Value);
            Assert.AreEqual(expectedSecondSchoolName, actual.AccessXml.Element("schools").Elements("school").Last().Attribute("name").Value);
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserAssociatedToThreeSchools_AndUserAssociatedToThreeRoles_AndUserAssociatedToThreeProviders_WhenCreateAccessChangeEvent_ThenSchoolsElementHasThreeSchoolElements_AndProvidersElementhasThreeProviderElements_AndRoleElementHasThreeRoleElements()
        {
            User user = new User
            {
                UserRoles = new List<UserRole>
                {
                    new UserRole
                    {
                        Role = new Role { Name = "whatever" },
                        Schools = new List<School>
                        {
                            new School { Name = "whatever" }
                        },
                        Providers = new List<Provider>
                        {
                            new Provider { Name = "whatever" }
                        }
                    },
                    new UserRole
                    {
                        Role = new Role { Name = "whatever" },
                        Schools = new List<School>
                        {
                            new School { Name = "whatever" }
                        },
                        Providers = new List<Provider>
                        {
                            new Provider { Name = "whatever" }
                        }
                    },
                    new UserRole
                    {
                        Role = new Role { Name = "whatever" },
                        Schools = new List<School>
                        {
                            new School { Name = "whatever" }
                        },
                        Providers = new List<Provider>
                        {
                            new Provider { Name = "whatever" }
                        }
                    }
                }
            };
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.AreEqual(3, actual.AccessXml.Element("roles").Elements("role").Count());
            Assert.AreEqual(3, actual.AccessXml.Element("schools").Elements("school").Count());
            Assert.AreEqual(3, actual.AccessXml.Element("providers").Elements("provider").Count());
        }

        [TestMethod]
        public void GivenUser_AndRequestor_AndUserHasNoAccesss_WhenCreateAccessChangeEvent_ThenReturnEmptyXml()
        {
            User user = new User();
            User requestor = new User();

            UserAccessChangeEvent actual = Target.CreateAccessChangeEvent(user, requestor);

            Assert.IsTrue(actual.AccessXml.IsEmpty);
        }

        [TestMethod]
        public void GivenNullUser_WhenCreateLoginEvent_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.CreateLoginEvent(null));
        }

        [TestMethod]
        public void GivenUser_WhenCreateLoginEvent_ThenReturnInstance()
        {
            User user = new User();

            LoginEvent actual = Target.CreateLoginEvent(user);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenUser_WhenCreateLoginEvent_ThenUserSet()
        {
            User user = new User { Id = 138 };

            LoginEvent actual = Target.CreateLoginEvent(user);

            Assert.AreEqual(user, actual.CreatingUser);
            Assert.AreEqual(user.Id, actual.CreatingUserId);
        }

        [TestMethod]
        public void GivenUser_WhenCreateLoginEvent_ThenTimeSet()
        {
            User user = new User();

            LoginEvent actual = Target.CreateLoginEvent(user);

            Assert.IsTrue(DateTime.Now.WithinTimeSpanOf(TimeSpan.FromSeconds(1), actual.CreateTime));
        }

        [TestMethod]
        public void GivenNullUser_WhenCreatePrivateHealthInfoViewEvent_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CreatePrivateHealthInfoViewEvent(null, new List<CustomFieldValue>()));
        }

        [TestMethod]
        public void GivenNullViewedValues_WhenCreatePrivateHealthInfoViewEvent_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.CreatePrivateHealthInfoViewEvent(new User(), null));
        }

        [TestMethod]
        public void GivenUser_AndViewedValues_WhenCreatePrivateHealthInfoViewEvent_ThenInstanceReturned()
        {
            User user = new User { Id = 138 };

            PrivateHealthDataViewEvent actual = Target.CreatePrivateHealthInfoViewEvent(user, new List<CustomFieldValue>());

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenUser_AndViewedValues_WhenCreatePrivateHealthInfoViewEvent_ThenUserSet()
        {
            User user = new User { Id = 138 };

            PrivateHealthDataViewEvent actual = Target.CreatePrivateHealthInfoViewEvent(user, new List<CustomFieldValue>());

            Assert.AreEqual(user, actual.CreatingUser);
            Assert.AreEqual(user.Id, actual.CreatingUserId);
        }

        [TestMethod]
        public void GivenUser_AndViewedValues_WhenCreatePrivateHealthInfoViewEvent_ThenTimeSet()
        {
            User user = new User();

            PrivateHealthDataViewEvent actual = Target.CreatePrivateHealthInfoViewEvent(user, new List<CustomFieldValue>());

            Assert.IsTrue(DateTime.Now.WithinTimeSpanOf(TimeSpan.FromSeconds(1), actual.CreateTime));
        }

        [TestMethod]
        public void GivenUser_AndViewedValues_WhenCreatePrivateHealthInfoViewEvent_ThenPhiValuesViewedSet()
        {
            User user = new User { Id = 138 };
            List<CustomFieldValue> expected = new List<CustomFieldValue> { new CustomFieldValue { Value = "blah" } };

            PrivateHealthDataViewEvent actual = Target.CreatePrivateHealthInfoViewEvent(user, expected);

            CollectionAssert.AreEqual(expected, actual.PhiValuesViewed);
        }
    }
}
