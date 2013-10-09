using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Web;

namespace SSD.ViewModels.DataTables
{   
    [TestClass]
    public class AuditLoginClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }
        private TestData TestData { get; set; }
        private AuditLoginClientDataTable Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockRepository.GenerateMock<HttpRequestBase>();
            TestData = new TestData();
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndUserMatchesId_WhenInvokeFilterPredicate_ThenReturnTrue()
        {
            LoginEvent item = new LoginEvent { CreatingUserId = 1 };
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditLoginClientDataTable(MockRequest);

            Assert.IsTrue(Target.FilterPredicate.Compile().Invoke(item));
        }

        [TestMethod]
        public void GivenIdRequestParameter_AndUserDoesntMatchId_WhenInvokeFilterPredicate_ThenReturnFalse()
        {
            LoginEvent item = new LoginEvent { CreatingUserId = 1 };
            MockRequest.Expect(m => m["id"]).Return("2");
            Target = new AuditLoginClientDataTable(MockRequest);

            Assert.IsFalse(Target.FilterPredicate.Compile().Invoke(item));
        }

        [TestMethod]
        public void GivenISortByCreatingUser_WhenSortSelector_ThenSortsByCreatingUser()
        {
            PrepareDataTableRequestParameters("0", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditLoginClientDataTable(MockRequest);
            string expected = TestData.UserAccessChangeEvents[0].CreatingUser.DisplayName;

            var actual = Target.SortSelector.Compile().Invoke(TestData.LoginEvents[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenISortByCreatedTime_WhenSortSelector_ThenSortsByCreatedTime()
        {
            PrepareDataTableRequestParameters("1", "asc");
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditLoginClientDataTable(MockRequest);
            string expected = ((int)TimeSpan.FromTicks(TestData.LoginEvents[0].CreateTime.Ticks).TotalMinutes).ToString();

            var actual = Target.SortSelector.Compile().Invoke(TestData.LoginEvents[0]);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenEntity_WhenInvokeDataSelector_ThenPropertiesMatch()
        {
            LoginEvent expectedState = TestData.LoginEvents[0];
            MockRequest.Expect(m => m["id"]).Return("1");
            Target = new AuditLoginClientDataTable(MockRequest);

            dynamic actual = Target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.CreateTime, actual.CreateTime);
            Assert.AreEqual(expectedState.CreatingUser.DisplayName, actual.CreatingUser);
            Assert.AreEqual(expectedState.Id, actual.Id);
        }

        private void PrepareDataTableRequestParameters(string sortColumn, string sortDirection)
        {
            MockRequest.Expect(m => m["iSortCol_0"]).Return(sortColumn);
            MockRequest.Expect(m => m["sSortDir_0"]).Return(sortDirection);
        }
    }
}
