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
    public class ServiceTypeRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<ServiceType> MockDbSet { get; set; }
        private ServiceTypeRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<ServiceType>>();
            MockContext.Expect(m => m.ServiceTypes).Return(MockDbSet);
            Target = new ServiceTypeRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceTypeRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceTypes_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAServiceType_WhenAdd_ThenAddToContext()
        {
            var expected = new ServiceType { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAServiceType_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new ServiceType { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAServiceType_WhenRemove_ThenRemoveFromContext()
        {
            var item = new ServiceType { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }

        [TestMethod]
        public void GivenAnUnassociatedServiceTypeAndCategory_WhenAddLink_ThenTheyAreAssociated()
        {
            ServiceType serviceType = new ServiceType();
            Category category = new Category();

            Target.AddLink(serviceType, category);

            CollectionAssert.Contains(serviceType.Categories.ToList(), category);
            CollectionAssert.Contains(category.ServiceTypes.ToList(), serviceType);
        }

        [TestMethod]
        public void GivenNullServiceType_WhenAddLink_ThenThrowException()
        {
            Category category = new Category();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(null, category));
        }

        [TestMethod]
        public void GivenNullCategory_WhenAddLink_ThenThrowException()
        {
            ServiceType serviceType = new ServiceType();

            Target.ExpectException<ArgumentNullException>(() => Target.AddLink(serviceType, null));
        }

        [TestMethod]
        public void GivenAnAssociatedServiceTypeAndCategory_WhenDeleteLink_TheyAreNoLongerAssociated()
        {

            ServiceType serviceType = new ServiceType();
            Category category = new Category { ServiceTypes = new List<ServiceType> { serviceType } };
            serviceType.Categories.Add(category);

            Target.DeleteLink(serviceType, category);

            CollectionAssert.DoesNotContain(serviceType.Categories.ToList(), category);
            CollectionAssert.DoesNotContain(category.ServiceTypes.ToList(), serviceType);
        }

        [TestMethod]
        public void GivenNullServiceType_WhenDeleteLink_ThenThrowException()
        {
            Category category = new Category();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(null, category));
        }

        [TestMethod]
        public void GivenNullCategory_WhenDeleteLink_ThenThrowException()
        {
            ServiceType serviceType = new ServiceType();

            Target.ExpectException<ArgumentNullException>(() => Target.DeleteLink(serviceType, null));
        }
    }
}
