using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class CustomFieldValueRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<CustomFieldValue> MockDbSet { get; set; }
        private CustomFieldValueRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<CustomFieldValue>>();
            MockContext.Expect(m => m.CustomFieldValues).Return(MockDbSet);
            Target = new CustomFieldValueRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new CustomFieldValueRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsCustomFieldValues_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACustomFieldValue_WhenAdd_ThenAddToContext()
        {
            var expected = new CustomFieldValue { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACustomFieldValue_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new CustomFieldValue { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACustomFieldValue_WhenDelete_ThenRemoveFromContext()
        {
            var item = new CustomFieldValue { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
