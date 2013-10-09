using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class DataTableBinderTest
    {
        private DataTableBinder Target { get; set; }
        private IClientDataTable<Program> DataTable { get; set; }
        private DataTableRequestModel RequestModel { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new DataTableBinder();
            DataTable = MockRepository.GenerateMock<IClientDataTable<Program>>();
            RequestModel = new DataTableRequestModel();
        }

        [TestMethod]
        public void GivenNullItems_WhenBind_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Bind<Program>(null, DataTable, RequestModel));
        }

        [TestMethod]
        public void GivenNullClientDataTable_WhenBind_ThenThrowException()
        {
            IQueryable<Program> items = Enumerable.Empty<Program>().AsQueryable();

            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Bind<Program>(items, null, RequestModel));
        }

        [TestMethod]
        public void GivenNullRequestModel_WhenBind_ThenThrowException()
        {
            IQueryable<Program> items = Enumerable.Empty<Program>().AsQueryable();

            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Bind<Program>(items, DataTable, null));
        }

        [TestMethod]
        public void WhenBind_ThenApplyFilter_AndThenApplySort_AndThenCreateResultSet()
        {
            IQueryable<Program> originalItems = Enumerable.Empty<Program>().AsQueryable();
            IQueryable<Program> filteredItems = new List<Program>().AsQueryable();
            IOrderedQueryable<Program> sortedItems = new List<Program>().AsQueryable().OrderBy(p => p);
            DataTable.Expect(m => m.ApplyFilters(originalItems)).Return(filteredItems);
            DataTable.Expect(m => m.ApplySort(filteredItems)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(sortedItems.Cast<object>().ToList());

            Target.Bind<Program>(originalItems, DataTable, RequestModel);

            DataTable.AssertWasCalled(m => m.CreateResultSet(sortedItems, RequestModel));
        }

        [TestMethod]
        public void GivenRequestEchoValue_WhenBind_ThenResultEchoMatches()
        {
            string expected = "this is the expected!";
            IQueryable<Program> items = Enumerable.Empty<Program>().AsQueryable();
            IQueryable<Program> sortedItems = new List<Program>().AsQueryable().OrderBy(p => p);
            RequestModel.sEcho = expected;
            DataTable.Expect(m => m.ApplySort(null)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(sortedItems.Cast<object>().ToList());

            DataTableResultModel actual = Target.Bind<Program>(items, DataTable, RequestModel);

            Assert.AreEqual(expected, actual.sEcho);
        }

        [TestMethod]
        public void GivenInitialRecords_WhenBind_ThenResultTotalRecordsMatches()
        {
            IQueryable<Program> originalItems = new List<Program> { new Program(), new Program(), new Program() }.AsQueryable();
            IQueryable<Program> sortedItems = new List<Program>().AsQueryable().OrderBy(p => p);
            DataTable.Expect(m => m.ApplySort(null)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(sortedItems.Cast<object>().ToList());

            DataTableResultModel actual = Target.Bind<Program>(originalItems, DataTable, RequestModel);

            Assert.AreEqual(originalItems.Count(), actual.iTotalRecords);
        }

        [TestMethod]
        public void GivenFilterLimitsResults_WhenBind_ThenResultTotalDisplayRecordsMatchesFilteredCount()
        {
            IQueryable<Program> originalItems = new List<Program> { new Program(), new Program(), new Program() }.AsQueryable();
            IQueryable<Program> filteredItems = new List<Program> { new Program() }.AsQueryable();
            IQueryable<Program> sortedItems = filteredItems.OrderBy(p => p);
            DataTable.Expect(m => m.ApplyFilters(originalItems)).Return(filteredItems);
            DataTable.Expect(m => m.ApplySort(filteredItems)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(sortedItems.Cast<object>().ToList());

            DataTableResultModel actual = Target.Bind<Program>(originalItems, DataTable, RequestModel);

            Assert.AreEqual(filteredItems.Count(), actual.iTotalDisplayRecords);
        }

        [TestMethod]
        public void GivenFilterLimitsResults_AndResultSetIsPaged_WhenBind_ThenResultTotalDisplayRecordsMatchesFilteredCount()
        {
            IQueryable<Program> originalItems = new List<Program> { new Program(), new Program(), new Program() }.AsQueryable();
            IQueryable<Program> filteredItems = new List<Program> { new Program(), new Program() }.AsQueryable();
            IQueryable<Program> sortedItems = filteredItems.OrderBy(p => p.Id);
            DataTable.Expect(m => m.ApplyFilters(originalItems)).Return(filteredItems);
            DataTable.Expect(m => m.ApplySort(filteredItems)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(sortedItems.Take(1).Cast<object>().ToList());

            DataTableResultModel actual = Target.Bind<Program>(originalItems, DataTable, RequestModel);

            Assert.AreEqual(filteredItems.Count(), actual.iTotalDisplayRecords);
        }

        [TestMethod]
        public void GivenCreateResultSetReturnsData_WhenBind_ThenResultDataMatches()
        {
            var expected = new List<object>();
            IQueryable<Program> originalItems = new List<Program> { new Program(), new Program(), new Program() }.AsQueryable();
            IOrderedQueryable<Program> sortedItems = new List<Program> { new Program() }.AsQueryable().OrderBy(p => p);
            DataTable.Expect(m => m.ApplySort(null)).Return(sortedItems);
            DataTable.Expect(m => m.CreateResultSet(sortedItems, RequestModel)).Return(expected);

            DataTableResultModel actual = Target.Bind<Program>(originalItems, DataTable, RequestModel);

            Assert.AreEqual(expected, actual.aaData);
        }
    }
}
