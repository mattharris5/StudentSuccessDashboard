using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSD.ViewModels.DataTables
{
    [TestClass]
    public class StudentApprovalClientDataTableTest
    {
        private HttpRequestBase MockRequest { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockRequest = MockHttpContextFactory.CreateRequest();
        }

        [TestMethod]
        public void GivenIDFilter_WhenConstruct_ThenIDFilterValueSet()
        {
            string expected = "324";
            MockRequest.Expect(m => m["ID"]).Return(expected);

            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.AreEqual(expected, target.Id);
        }

        [TestMethod]
        public void GivenFirstNameFilter_WhenConstruct_ThenFirstNameFilterValueSet()
        {
            string expected = "bob";
            MockRequest.Expect(m => m["firstName"]).Return(expected);

            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.AreEqual(expected, target.FirstName);
        }

        [TestMethod]
        public void GivenLastNameFilter_WhenConstruct_ThenLastNameFilterValueSet()
        {
            string expected = "smith";
            MockRequest.Expect(m => m["lastName"]).Return(expected);

            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.AreEqual(expected, target.LastName);
        }

        [TestMethod]
        public void GivenSchoolListFilter_WhenConstruct_ThenSchoolFilterListSet()
        {
            string[] expected = { "high school", "middle school", "other school" };
            MockRequest.Expect(m => m["schools"]).Return("high school|middle school|other school");

            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            CollectionAssert.AreEqual(expected, target.Schools.ToList());
        }

        [TestMethod]
        public void GivenProviderListFilter_WhenConstruct_ThenProviderFilterListSet()
        {
            string[] expected = { "YMCA", "Learning Place", "Athletics Club" };
            MockRequest.Expect(m => m["providers"]).Return("|YMCA|Learning Place|Athletics Club|");

            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            CollectionAssert.AreEqual(expected, target.Providers.ToList());
        }

        [TestMethod]
        public void GivenNoSortColumn_WhenExecuteSortSelector_ThenUseDefaultSortById()
        {
            List<Student> toSort = CreateStudentList();
            List<Student> expected = new List<Student> { toSort[3], toSort[4], toSort[1], toSort[0], toSort[2], toSort[6], toSort[5] };
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);
            var targetSorter = target.SortSelector.Compile();

            List<Student> actual = toSort.OrderBy(targetSorter).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenSortColumnIndex1_WhenExecuteSortSelector_ThenSortByName()
        {
            List<Student> toSort = CreateStudentList();
            List<Student> expected = new List<Student> { toSort[2], toSort[6], toSort[0], toSort[3], toSort[5], toSort[1], toSort[4] };
            MockRequest.Expect(m => m["iSortCol_0"]).Return("1");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);
            var targetSorter = target.SortSelector.Compile();

            List<Student> actual = toSort.OrderBy(targetSorter).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenSortColumnIndex2_WhenExecuteSortSelector_ThenSortByGrade()
        {
            List<Student> toSort = CreateStudentList();
            List<Student> expected = new List<Student> { toSort[1], toSort[2], toSort[4], toSort[6], toSort[5], toSort[0], toSort[3] };
            MockRequest.Expect(m => m["iSortCol_0"]).Return("2");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);
            var targetSorter = target.SortSelector.Compile();

            List<Student> actual = toSort.OrderBy(targetSorter).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenSortColumnIndex3_WhenExecuteSortSelector_ThenSortBySchoolName()
        {
            List<Student> toSort = CreateStudentList();
            List<Student> expected = new List<Student> { toSort[0], toSort[3], toSort[2], toSort[4], toSort[1], toSort[5], toSort[6] };
            MockRequest.Expect(m => m["iSortCol_0"]).Return("3");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);
            var targetSorter = target.SortSelector.Compile();

            List<Student> actual = toSort.OrderBy(targetSorter).ToList();

            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenStudent_WhenExecuteDataSelctor_ThenDataContainsStudentProperties()
        {
            Student expectedState = new Student
            {
                Id = 1877,
                FirstName = "Bob",
                MiddleName = "Allan",
                LastName = "Smith",
                HasParentalOptOut = true,
                Grade = 7,
                School = new School { Name = "Random High School" },
                StudentSISId = "3823282"
            };
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(expectedState);

            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedState.FullName, actual.Name);
            Assert.AreEqual(expectedState.HasParentalOptOut, actual.HasParentalOptOut);
            Assert.AreEqual(expectedState.Grade, actual.Grade);
            Assert.AreEqual(expectedState.School.Name, actual.School);
            Assert.AreEqual(expectedState.StudentSISId, actual.StudentSISId);
        }

        [TestMethod]
        public void GivenStudent_AndStudentHasApprovedProviders_WhenExecuteDataSelctor_ThenDataContainsApprovedProviderDetail()
        {
            List<Provider> expectedState = new List<Provider> { new Provider { Id = 382, Name = "Blah1", IsActive = true }, new Provider { Id = 289, Name = "Grapejuice", IsActive = true } };
            Student student = new Student
            {
                School = new School(),
                ApprovedProviders = expectedState
            };
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(student);

            int i = 0;
            foreach (dynamic actualProvider in actual.ApprovedProviders)
            {
                Assert.AreEqual(expectedState[i].Id, actualProvider.Id);
                Assert.AreEqual(expectedState[i].Name, actualProvider.Name);
                i++;
            }
            Assert.AreEqual(expectedState.Count, i);
        }

        [TestMethod]
        public void GivenStudent_AndStudentHasApprovedProviders_AndSomeProvidersAreInactive_WhenExecuteDataSelctor_ThenDataContainsActiveApprovedProviderDetail()
        {
            Provider expectedState = new Provider { Id = 382, Name = "Blah1", IsActive = true };
            Student student = new Student
            {
                School = new School(),
                ApprovedProviders = new List<Provider> { expectedState, new Provider { Id = 289, Name = "Grapejuice" } }
            };
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            dynamic actual = target.DataSelector.Compile().Invoke(student);

            dynamic actualApprovedProvider = ((IEnumerable<object>)actual.ApprovedProviders).Single();
            Assert.AreEqual(expectedState.Id, actualApprovedProvider.Id);
            Assert.AreEqual(expectedState.Name, actualApprovedProvider.Name);
        }

        [TestMethod]
        public void GivenIDFilter_AndStudentMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { StudentSISId = "1234", School = new School() };
            MockRequest.Expect(m => m["ID"]).Return("23");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenIDFilter_AndStudentDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Student toFilter = new Student { StudentSISId = "1234" };
            MockRequest.Expect(m => m["ID"]).Return("45");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenLastNameFilter_AndStudentMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { LastName = "abcd", School = new School() };
            MockRequest.Expect(m => m["lastName"]).Return("bc");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenLastNameFilter_AndStudentMatches_AndLastNameCaseDiffers_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { LastName = "abCd", School = new School() };
            MockRequest.Expect(m => m["lastName"]).Return("Bc");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenLastNameFilter_AndStudentDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Student toFilter = new Student { LastName = "abcd" };
            MockRequest.Expect(m => m["lastName"]).Return("de");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenFirstNameFilter_AndStudentMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { FirstName = "abcd", School = new School() };
            MockRequest.Expect(m => m["firstName"]).Return("bc");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenFirstNameFilter_AndStudentMatches_AndFirstNameCaseDiffers_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { FirstName = "aBcd", School = new School() };
            MockRequest.Expect(m => m["firstName"]).Return("bC");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenFirstNameFilter_AndStudentDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Student toFilter = new Student { FirstName = "abcd" };
            MockRequest.Expect(m => m["firstName"]).Return("de");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenSchoolFilterList_AndStudentSchoolMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { School = new School { Name = "def" } };
            MockRequest.Expect(m => m["schools"]).Return("abc|def|ghi");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenSchoolFilterList_AndStudentSchoolDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Student toFilter = new Student { School = new School { Name = "jkl" } };
            MockRequest.Expect(m => m["schools"]).Return("abc|def|ghi");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenProviderFilterList_AndStudentSchoolMatches_WhenExecuteFilterPredicate_ThenReturnTrue()
        {
            Student toFilter = new Student { ApprovedProviders = new List<Provider> { new Provider { Name = "def" } }, School = new School() };
            MockRequest.Expect(m => m["providers"]).Return("abc|def|ghi");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsTrue(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        [TestMethod]
        public void GivenProviderFilterList_AndStudentSchoolDoesNotMatch_WhenExecuteFilterPredicate_ThenReturnFalse()
        {
            Student toFilter = new Student { ApprovedProviders = new List<Provider> { new Provider { Name = "jkl" } }, School = new School() };
            MockRequest.Expect(m => m["providers"]).Return("abc|def|ghi");
            StudentApprovalClientDataTable target = new StudentApprovalClientDataTable(MockRequest);

            Assert.IsFalse(target.FilterPredicate.Compile().Invoke(toFilter));
        }

        private List<Student> CreateStudentList()
        {
            return new List<Student>
            {
                new Student { Id = 1, StudentSISId = "4", LastName = "B", FirstName = "A", MiddleName = "B", Grade = 6, School = new School { Name = "A" } }, // 0
                new Student { Id = 2, StudentSISId = "3", LastName = "D", FirstName = "A", MiddleName = "A", Grade = 1, School = new School { Name = "E" } }, // 1
                new Student { Id = 3, StudentSISId = "5", LastName = "A", FirstName = "C", MiddleName = "A", Grade = 2, School = new School { Name = "C" } }, // 2
                new Student { Id = 4, StudentSISId = "1", LastName = "B", FirstName = "B", MiddleName = "Z", Grade = 7, School = new School { Name = "B" } }, // 3
                new Student { Id = 5, StudentSISId = "2", LastName = "E", FirstName = "A", MiddleName = "A", Grade = 3, School = new School { Name = "D" } }, // 4
                new Student { Id = 6, StudentSISId = "7", LastName = "C", FirstName = "A", MiddleName = "A", Grade = 5, School = new School { Name = "F" } }, // 5
                new Student { Id = 7, StudentSISId = "6", LastName = "B", FirstName = "A", MiddleName = "A", Grade = 4, School = new School { Name = "G" } }  // 6
            };
        }
    }
}
