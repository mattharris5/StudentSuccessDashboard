using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class PrivateHealthFieldClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }

        [TestInitialize]
        public void IntializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeSortSelector_ThenSortOnName()
        {
            string expected = "this is what I want!";
            PrivateHealthField customField = new PrivateHealthField { Name = expected };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPrivateHealthField_AndSortColumnIndexIs1_WhenInvokeSortSelector_ThenSortOnFieldType()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("1");
            string expected = "this is what I want!";
            PrivateHealthField customField = new PrivateHealthField { Name = "this is NOT what I want!!!!", CustomFieldType = new CustomFieldType { Name = expected } };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPrivateHealthField_AndProviderIsNull_AndSortColumnIndexIs2_WhenInvokeSortSelector_ThenSortOnEmptyString()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("2");
            string expected = string.Empty;
            PrivateHealthField customField = new PrivateHealthField { Name = "this is NOT what I want!!!!", CustomFieldType = new CustomFieldType { Name = "this is NOT what I want!!!!" } };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPrivateHealthField_AndProviderIsNotNull_AndSortColumnIndexIs2_WhenInvokeSortSelector_ThenSortOnProviderName()
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return("2");
            string expected = "this is the value I want";
            PrivateHealthField customField = new PrivateHealthField { CustomFieldType = new CustomFieldType(), Provider = new Provider { Name = expected } };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            var actual = target.SortSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeDataSelector_ThenIdPropertyMatches()
        {
            int expected = 7438095;
            PrivateHealthField customField = new PrivateHealthField
            {
                Id = expected,
                CustomFieldType = new CustomFieldType()
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Id);
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeDataSelector_ThenNamePropertyMatches()
        {
            string expected = "this is what I want!";
            PrivateHealthField customField = new PrivateHealthField
            {
                Name = expected,
                CustomFieldType = new CustomFieldType()
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Name);
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeDataSelector_ThenTypePropertyMatches()
        {
            string expected = "field type";
            PrivateHealthField customField = new PrivateHealthField
            {
                CustomFieldType = new CustomFieldType { Name = expected }
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Type);
        }

        [TestMethod]
        public void GivenPrivateHealthField_AndProviderNotNull_WhenInvokeDataSelector_ThenProviderPropertyMatches()
        {
            string expected = "provider name";
            PrivateHealthField customField = new PrivateHealthField
            {
                CustomFieldType = new CustomFieldType(),
                Provider = new Provider { Name = expected }
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(expected, actual.Provider);
        }

        [TestMethod]
        public void GivenPrivateHealthField_AndProviderNotNull_WhenInvokeDataSelector_ThenProviderPropertyEmpty()
        {
            PrivateHealthField customField = new PrivateHealthField
            {
                CustomFieldType = new CustomFieldType()
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            Assert.AreEqual(string.Empty, actual.Provider);
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeDataSelector_ThenCategoriesPropertyMatches()
        {
            string[] expected = new[] { "category1", "category2", "category3" };
            PrivateHealthField customField = new PrivateHealthField
            {
                Categories = expected.Select(c => new CustomFieldCategory { Name = c }).ToList(),
                CustomFieldType = new CustomFieldType()
            };
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(customField);

            CollectionAssert.AreEqual(expected, ((IEnumerable<string>)actual.Categories).ToList());
        }

        [TestMethod]
        public void GivenPrivateHealthField_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(new PrivateHealthField()));
        }

        [TestMethod]
        public void GivenPublicField_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            PrivateHealthFieldClientDataTable target = new PrivateHealthFieldClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(new PublicField()));
        }
    }
}
