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
    public class ServiceOfferingRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<ServiceOffering> MockDbSet { get; set; }
        private ServiceOfferingRepository Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<ServiceOffering>>();
            MockContext.Expect(m => m.ServiceOfferings).Return(MockDbSet);
            Target = new ServiceOfferingRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceOfferings_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAServiceOffering_WhenAdd_ThenAddToContext()
        {
            var expected = new ServiceOffering { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAServiceOffering_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new ServiceOffering { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAServiceOffering_WhenRemove_ThenThrowException()
        {
            var item = new ServiceOffering { Id = 1 };

            Target.ExpectException<NotSupportedException>(() => Target.Remove(item));
        }

        [TestMethod]
        public void GivenAnAssociatedServiceOfferingAndUser_WhenAddLink_ThenTheyAreAssociated()
        {
            ServiceOffering serviceOffering = new ServiceOffering();
            User user = new User();

            Target.AddLink(serviceOffering, user);

            CollectionAssert.Contains(serviceOffering.UsersLinkingAsFavorite.ToList(), user);
            CollectionAssert.Contains(user.FavoriteServiceOfferings.ToList(), serviceOffering);
        }

        [TestMethod]
        public void GivenNullServiceOffering_WhenAddLink_ThenThrowException()
        {
            User user = new User();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(null, user));
        }

        [TestMethod]
        public void GivenNullUser_WhenAddLink_ThenThrowException()
        {
            ServiceOffering serviceOffering = new ServiceOffering();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(serviceOffering, null));
        }

        [TestMethod]
        public void GivenAnAssociatedUserAndServiceOffering_WhenDeleteLink_ThenTheyAreNoLongerAssociated()
        {
            ServiceOffering serviceOffering = new ServiceOffering();
            User user = new User { FavoriteServiceOfferings = new List<ServiceOffering> { serviceOffering } };
            serviceOffering.UsersLinkingAsFavorite.Add(user);

            Target.DeleteLink(serviceOffering, user);

            CollectionAssert.DoesNotContain(serviceOffering.UsersLinkingAsFavorite.ToList(), user);
            CollectionAssert.DoesNotContain(user.FavoriteServiceOfferings.ToList(), serviceOffering);
        }

        [TestMethod]
        public void GivenNullServiceOffering_WhenDeleteLink_ThenThrowException()
        {
            User user = new User();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(null, user));
        }

        [TestMethod]
        public void GivenNullUser_WhenDeleteLink_ThenThrowException()
        {
            ServiceOffering serviceOffering = new ServiceOffering();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(serviceOffering, null));
        }
    }
}
