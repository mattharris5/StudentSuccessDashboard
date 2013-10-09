using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class CustomFieldCategoryRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<CustomFieldCategory> MockDbSet { get; set; }
        private CustomFieldCategoryRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<CustomFieldCategory>>();
            MockContext.Expect(m => m.CustomFieldCategories).Return(MockDbSet);
            Target = new CustomFieldCategoryRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new CustomFieldCategoryRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsCustomFieldCategories_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACustomFieldCategory_WhenAdd_ThenAddToContext()
        {
            var expected = new CustomFieldCategory { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACustomFieldCategory_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new CustomFieldCategory { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACustomFieldCategory_WhenRemove_ThenRemoveFromContext()
        {
            var item = new CustomFieldCategory { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
