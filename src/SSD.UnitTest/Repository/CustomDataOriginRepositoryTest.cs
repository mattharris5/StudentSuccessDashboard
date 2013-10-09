using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class CustomDataOriginRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<CustomDataOrigin> MockDbSet { get; set; }
        private ICustomDataOriginRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<CustomDataOrigin>>();
            MockContext.Expect(m => m.CustomDataOrigins).Return(MockDbSet);
            Target = new CustomDataOriginRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new CustomDataOriginRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsCustomDataOrigins_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACustomDataOrigin_WhenAdd_ThenAddToContext()
        {
            var expected = new CustomDataOrigin { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACustomDataOrigin_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new CustomDataOrigin { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACustomDataOrigin_WhenRemove_ThenRemoveFromContext()
        {
            var item = new CustomDataOrigin { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
