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
    public class ProviderRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Provider> MockDbSet { get; set; }
        private ProviderRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Provider>>();
            MockContext.Expect(m => m.Providers).Return(MockDbSet);
            Target = new ProviderRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ProviderRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsProviders_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAProvider_WhenAdd_ThenAddToContext()
        {
            var expected = new Provider { Id = 1 };
            
            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAProvider_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Provider { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAProvider_WhenRemove_ThenRemoveFromContext()
        {
            var item = new Provider { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
