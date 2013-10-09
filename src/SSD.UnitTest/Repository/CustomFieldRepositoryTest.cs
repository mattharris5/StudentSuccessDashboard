using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Data;
using SSD.Domain;
using System;
using System.Data.Entity;

namespace SSD.Repository
{
    [TestClass]
    public class CustomFieldRepositoryTest
    {
        private IEducationContext MockContext { get; set; }
        private IDbSet<CustomField> MockDbSet { get; set; }
        private CustomFieldRepository Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockContext = MockRepository.GenerateMock<IEducationContext>();
            MockDbSet = MockRepository.GenerateMock<IDbSet<CustomField>>();
            MockContext.Expect(m => m.CustomFields).Return(MockDbSet);
            Target = new CustomFieldRepository(MockContext);
        }

        [TestMethod]
        public void GivenNullContext_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new CustomFieldRepository(null));
        }

        [TestMethod]
        public void GivenContextContainsCustomFields_WhenGetItems_ThenAllContextItemsAreReturned()
        {
            var actual = Target.Items;

            Assert.AreEqual(MockDbSet, actual);
        }

        [TestMethod]
        public void GivenACustomField_WhenAdd_ThenAddToContext()
        {
            var expected = new PublicField { Id = 1 };

            Target.Add(expected);

            MockDbSet.AssertWasCalled(m => m.Add(expected));
        }

        [TestMethod]
        public void GivenACustomField_WhenUpdate_ThenContextSetsModified()
        {
            var expected = new PublicField { Id = 1 };

            Target.Update(expected);

            MockContext.AssertWasCalled(m => m.SetModified(expected));
        }

        [TestMethod]
        public void GivenACustomField_WhenRemove_ThenRemoveFromContext()
        {
            var item = new PublicField { Id = 1 };

            Target.Remove(item);

            MockDbSet.AssertWasCalled(m => m.Remove(item));
        }
    }
}
