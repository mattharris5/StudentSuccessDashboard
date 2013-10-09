using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class CustomFieldTypeRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<CustomFieldType> MockDbSet { get; set; }
        private CustomFieldTypeRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<CustomFieldType>>();
            MockContext.Expect(m => m.CustomFieldTypes).Return(MockDbSet);
            Target = new CustomFieldTypeRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new CustomFieldTypeRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsCustomFieldTypes_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACustomFieldType_WhenAdd_ThenAddToContext()
        {
            var expected = new CustomFieldType { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACustomFieldType_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new CustomFieldType { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACustomFieldType_WhenDelete_ThenRemoveFromContext()
        {
            var item = new CustomFieldType { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
