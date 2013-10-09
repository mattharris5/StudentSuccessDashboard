using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class PropertyRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Property> MockDbSet { get; set; }
        private PropertyRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Property>>();
            MockContext.Expect(m => m.Properties).Return(MockDbSet);
            Target = new PropertyRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new PropertyRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsProperties_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenAProperty_WhenAdd_ThenAddToContext()
        {
            var expected = new Property { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenAProperty_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Property { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenAProperty_WhenDelete_ThenRemoveFromContext()
        {
            var item = new Property { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
