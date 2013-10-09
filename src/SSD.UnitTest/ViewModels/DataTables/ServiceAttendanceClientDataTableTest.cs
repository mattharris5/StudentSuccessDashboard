using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class ServiceAttendanceClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private TestData TestData { get; set; }
        private ServiceAttendanceClientDataTable Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenISortByDate_WhenSortSelector_ThenSortsByDate()
        {
            PrepareDataTableRequestParameters("0", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);
            string expected = ((int)TimeSpan.FromTicks(TestData.ServiceAttendances[0].DateAttended.Ticks).TotalMinutes).ToString();

            var actual = Target.SortSelector.Compile().Invoke(TestData.ServiceAttendances[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenISortBySubject_WhenSortSelector_ThenSortsBySubjectName()
        {
            PrepareDataTableRequestParameters("1", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);
            string expected = TestData.ServiceAttendances[0].Subject.Name;

            var actual = Target.SortSelector.Compile().Invoke(TestData.ServiceAttendances[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenISortByDuration_WhenSortSelector_ThenSortsByDuration()
        {
            PrepareDataTableRequestParameters("2", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);
            string expected = TestData.ServiceAttendances[0].Duration.ToString();

            var actual = Target.SortSelector.Compile().Invoke(TestData.ServiceAttendances[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndServiceAttendanceMatchesStudentAssignedOfferingId_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            ServiceAttendance attendance = new ServiceAttendance { StudentAssignedOfferingId = 1 };
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(attendance));
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndServiceAttendanceDoesNotMatchStudentAssignedOfferingId_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            ServiceAttendance attendance = new ServiceAttendance { StudentAssignedOfferingId = 40 };
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(attendance));
        }

        [TestMethod]
        public void GivenServiceAttendance_WhenInvokeDataSelector_ThenPropertiesMatch()
        {
            ServiceAttendance expectedState = new ServiceAttendance
            {
                DateAttended = new DateTime(2003, 4, 5),
                Duration = 45,
                Id = 35,
                Notes = "blkah",
                Subject = new Subject { Name = "disojfw" }
            };
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new ServiceAttendanceClientDataTable(MockRequest);

            dynamic actual = Target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.DateAttended, actual.DateAttended);
            Assert.AreEqual(expectedState.Duration, actual.Duration);
            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedState.Notes, actual.Notes);
            Assert.AreEqual(expectedState.Subject.Name, actual.Subject);
        }

        private void PrepareDataTableRequestParameters(string sortColumn, string sortDirection)
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return(sortColumn);
            MockRequest.Expect(m => m["sSortDir_0"]).Return(sortDirection);
        }
    }
}
