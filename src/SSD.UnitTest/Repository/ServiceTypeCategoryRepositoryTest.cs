using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class ServiceTypeCategoryRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<Category> MockDbSet { get; set; }
        private ServiceTypeCategoryRepository Target { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<Category>>();
            MockContext.Expect(m => m.Categories).Return(MockDbSet);
            Target = new ServiceTypeCategoryRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowEception()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceTypeCategoryRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsServiceTypeCategories_WhenGetItmes_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACategory_WhenAdd_ThenAddToContext()
        {
            var expected = new Category { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACategory_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new Category { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACategory_WhenRemove_ThenRemoveFromContext()
        {
            var item = new Category { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
