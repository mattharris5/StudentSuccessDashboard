using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class BaseClientDataTableTest
    {
        [TestMethod]
        public void GivenNullRequest_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new TestClientDataTable(null));
        }

        [TestMethod]
        public void GivenRequestContainsSortColumnAndSortDirection_WhenConstruct_ThenHasSortIsTrue()
        {
            TestClientDataTable target = CreateTarget(1, "asc");

            Assert.IsTrue(target.HasSort);
        }

        [TestMethod]
        public void GivenItems_WhenApplyFilters_ThenItemsGetFilteredByPredicateApplied()
        {
            TestClientDataTable target = CreateTarget(1, "asc");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();

            var actual = target.ApplyFilters(items);

            CollectionAssert.AreEqual(new[] { "yo" }, actual.ToList());
        }

        [TestMethod]
        public void GivenItems_WhenApplySort_ThenItemsGetSortedBySortSelector()
        {
            TestClientDataTable target = CreateTarget(1, "asc");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();

            var actual = target.ApplySort(items);

            CollectionAssert.AreEqual(new[] { "hi", "sup", "yo" }, actual.ToList());
        }

        [TestMethod]
        public void GivenItems_AndHasSortFalse_WhenApplySort_ThenItemsUnchanged()
        {
            TestClientDataTable target = CreateTarget(1, null);
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();

            var actual = target.ApplySort(items);

            CollectionAssert.AreEqual(new[] { "hi", "yo", "sup" }, actual.ToList());
        }

        [TestMethod]
        public void GivenItems_AndHasSortNotAscending_WhenApplySort_ThenItemsSortedDescending()
        {
            TestClientDataTable target = CreateTarget(1, "not 'asc'");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();

            var actual = target.ApplySort(items);

            CollectionAssert.AreEqual(new[] { "yo", "sup", "hi" }, actual.ToList());
        }

        [TestMethod]
        public void GivenItems_WhenCreateResultSet_ThenItemsGetSelectedByColumnArraySelector()
        {
            TestClientDataTable target = CreateTarget(1, "asc");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();
            DataTableRequestModel requestModel = new DataTableRequestModel { iDisplayLength = -1 };

            var actual = target.CreateResultSet(items, requestModel);

            actual.Cast<string[]>().ToList().AssertItemsEqual(new[] { new[] { "2", "hi" }, new[] { "2", "yo" }, new[] { "3", "sup" } });
        }

        [TestMethod]
        public void GivenItems_AndDisplayLengthSet_WhenCreateResultSet_ThenItemsCountDoesNotExceedDisplayLength()
        {
            TestClientDataTable target = CreateTarget(1, "asc");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();
            DataTableRequestModel requestModel = new DataTableRequestModel { iDisplayLength = 2 };

            var actual = target.CreateResultSet(items, requestModel);

            actual.Cast<string[]>().ToList().AssertItemsEqual(new[] { new[] { "2", "hi" }, new[] { "2", "yo" } });
        }

        [TestMethod]
        public void GivenNullRequestModel_WhenCreateResultSet_ThenThrowException()
        {
            TestClientDataTable target = CreateTarget(1, "asc");
            IQueryable<string> items = new[] { "hi", "yo", "sup" }.AsQueryable();

            target.ExpectException<ArgumentNullException>(() => target.CreateResultSet(items, null));
        }

        private TestClientDataTable CreateTarget(int? sortColumnIndex, string sortDirection)
        {
            HttpRequestBase request = MockHttpContextFactory.CreateRequest();
            request.Expect(m => m["iSortCol_0"]).Return(sortColumnIndex.HasValue ? sortColumnIndex.Value.ToString() : null);
            request.Expect(m => m["sSortDir_0"]).Return(sortDirection);
            return new TestClientDataTable(request);
        }

        private class TestClientDataTable : BaseClientDataTable<string>
        {
            public TestClientDataTable(HttpRequestBase request)
                : base(request)
            { }

            public override Expression<Func<string, string>> SortSelector
            {
                get { return item => item; }
            }

            public override Expression<Func<string, object>> DataSelector
            {
                get { return item => new[] { item.Length.ToString(), item }; }
            }

            public override Expression<Func<string, bool>> FilterPredicate
            {
                get { return item => item == "yo"; }
            }
        }
    }
}
