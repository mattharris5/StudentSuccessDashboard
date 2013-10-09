using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class PublicFieldClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }

        [TestInitialize]
        public void IntializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeSortSelector_ThenSortOnName()
        {
            string expected = "this is what I want!";
            PublicField customField = new PublicField { Name = expected };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPublicField_AndSortColumnIndexIs1_WhenInvokeSortSelector_ThenSortOnFieldType()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("1");
            string expected = "this is what I want!";
            PublicField customField = new PublicField { Name = "this is NOT what I want!!!!", CustomFieldType = new CustomFieldType { Name = expected } };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeDataSelector_ThenIdPropertyMatches()
        {
            int expected = 7438095;
            PublicField customField = new PublicField
            {
                Id = expected,
                CustomFieldType = new CustomFieldType()
            };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Id);
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeDataSelector_ThenNamePropertyMatches()
        {
            string expected = "this is what I want!";
            PublicField customField = new PublicField
            {
                Name = expected,
                CustomFieldType = new CustomFieldType()
            };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Name);
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeDataSelector_ThenTypePropertyMatches()
        {
            string expected = "field type";
            PublicField customField = new PublicField
            {
                CustomFieldType = new CustomFieldType { Name = expected }
            };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Type);
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeDataSelector_ThenCategoriesPropertyMatches()
        {
            string[] expected = new[] { "category1", "category2", "category3" };
            PublicField customField = new PublicField
            {
                Categories = expected.Select(c => new CustomFieldCategory { Name = c }).ToList(),
                CustomFieldType = new CustomFieldType()
            };
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Categories).ToList());
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(new PublicField()));
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            PublicFieldClientDataTable target = new PublicFieldClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(new PrivateHealthField()));
        }
    }
}
