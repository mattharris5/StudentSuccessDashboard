using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Web;

namespace SSD.ViewModels.DataTables
{   
    [TestClass]
    public class AuditAccessClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private TestData TestData { get; set; }
        private AuditAccessClientDataTable Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndUserMatchesId_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            UserAccessChangeEvent changeEvent = new UserAccessChangeEvent { UserId = 1 };
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditAccessClientDataTable(MockRequest);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(changeEvent));
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndUserDoesntMatchId_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            UserAccessChangeEvent changeEvent = new UserAccessChangeEvent { UserId = 1 };
            MockRequest.Expect(m => m["id"]).Return("2");
            Target = new AuditAccessClientDataTable(MockRequest);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(changeEvent));
        }

        [TestMethod]
        public void GivenISortByCreatingUser_WhenSortSelector_ThenSortsByCreatingUser()
        {
            PrepareDataTableRequestParameters("0", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditAccessClientDataTable(MockRequest);
            string expected = TestData.UserAccessChangeEvents[0].CreatingUser.DisplayName;

            var actual = Target.SortSelector.Compile().Invoke(TestData.UserAccessChangeEvents[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenISortByCreatedTime_WhenSortSelector_ThenSortsByCreatedTime()
        {
            PrepareDataTableRequestParameters("1", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditAccessClientDataTable(MockRequest);
            string expected = ((int)TimeSpan.FromTicks(TestData.UserAccessChangeEvents[0].CreateTime.Ticks).TotalMinutes).ToString();

            var actual = Target.SortSelector.Compile().Invoke(TestData.UserAccessChangeEvents[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenUserAccessChangeEvent_WhenInvokeDataSelector_ThenPropertiesMatch()
        {
            UserAccessChangeEvent expectedState = TestData.UserAccessChangeEvents[0];
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditAccessClientDataTable(MockRequest);

            dynamic actual = Target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.CreateTime, actual.CreateTime);
            Assert.AreEqual(expectedState.CreatingUser.DisplayName, actual.CreatingUser);
            Assert.AreEqual(expectedState.UserActive, actual.UserActive);
            Assert.AreEqual(expectedState.AccessData, actual.AccessData);
            Assert.AreEqual(expectedState.Id, actual.Id);
        }

        private void PrepareDataTableRequestParameters(string sortColumn, string sortDirection)
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return(sortColumn);
            MockRequest.Expect(m => m["sSortDir_0"]).Return(sortDirection);
        }
    }
}
