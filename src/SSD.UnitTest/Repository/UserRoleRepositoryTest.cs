using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Repository
{
    [TestClass]
    public class UserRoleRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<UserRole> MockDbSet { get; set; }
        private UserRoleRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<UserRole>>();
            MockContext.Expect(m => m.UserRoles).Return(MockDbSet);
            Target = new UserRoleRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new UserRoleRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsUserRoles_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAUserRole_WhenAdd_ThenAddToContext()
        {
            var expected = new UserRole { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAUserRole_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new UserRole { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAUserRole_WhenRemove_ThenRemoveFromContext()
        {
            var item = new UserRole { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }

        [TestMethod]
        public void GivenAnUnassociatedUserRoleAndProvider_WhenAddLink_ThenTheyAreAssociated()
        {
            UserRole userRole = new UserRole();
            Provider provider = new Provider();

            Target.AddLink(userRole, provider);

            CollectionAssert.Contains(userRole.Providers.ToList(), provider);
            CollectionAssert.Contains(provider.UserRoles.ToList(), userRole);
        }

        [TestMethod]
        public void GivenAnUnassociatedUserRoleAndSchool_WhenAddLink_ThenTheyAreAssociated()
        {
            UserRole userRole = new UserRole();
            School school = new School();

            Target.AddLink(userRole, school);

            CollectionAssert.Contains(userRole.Schools.ToList(), school);
            CollectionAssert.Contains(school.UserRoles.ToList(), userRole);
        }

        [TestMethod]
        public void GivenNullUserRole_AndProvider_WhenAddLink_ThenThrowException()
        {
            Provider provider = new Provider();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(null, provider));
        }

        [TestMethod]
        public void GivenNullProvider_WhenAddLink_ThenThrowException()
        {
            UserRole userRole = new UserRole();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(userRole, (Provider)null));
        }

        [TestMethod]
        public void GivenNullUserRole_AndSchool_WhenAddLink_ThenThrowException()
        {
            School school = new School();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(null, school));
        }

        [TestMethod]
        public void GivenNullSchool_WhenAddLink_ThenThrowException()
        {
            UserRole userRole = new UserRole();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(userRole, (School)null));
        }

        [TestMethod]
        public void GivenAnAssociatedUserRoleAndProvider_WhenDeleteLink_ThenTheyAreNoLongerAssociated()
        {
            UserRole userRole = new UserRole();
            Provider provider = new Provider { UserRoles = new List<UserRole> { userRole } };
            userRole.Providers.Add(provider);

            Target.DeleteLink(userRole, provider);

            CollectionAssert.DoesNotContain(userRole.Providers.ToList(), provider);
            CollectionAssert.DoesNotContain(provider.UserRoles.ToList(), userRole);
        }

        [TestMethod]
        public void GivenAnAssociatedUserRoleAndSchool_WhenDeleteLink_ThenTheyAreNoLongerAssociated()
        {
            UserRole userRole = new UserRole();
            School school = new School { UserRoles = new List<UserRole> { userRole } };
            userRole.Schools.Add(school);

            Target.DeleteLink(userRole, school);

            CollectionAssert.DoesNotContain(userRole.Schools.ToList(), school);
            CollectionAssert.DoesNotContain(school.UserRoles.ToList(), userRole);
        }

        [TestMethod]
        public void GivenNullUserRole_AndProvider_WhenDeleteLink_ThenThrowException()
        {
            Provider provider = new Provider();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(null, provider));
        }

        [TestMethod]
        public void GivenNullProvider_WhenDeleteLink_ThenThrowException()
        {
            UserRole userRole = new UserRole();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(userRole, (Provider)null));
        }

        [TestMethod]
        public void GivenNullUserRole_AndSchool_WhenDeleteLink_ThenThrowException()
        {
            School school = new School();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(null, school));
        }

        [TestMethod]
        public void GivenNullSchool_WhenDeleteLink_ThenThrowException()
        {
            UserRole userRole = new UserRole();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(userRole, (School)null));
        }
    }
}
