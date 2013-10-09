using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Business;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace SSD.Controllers
{
    [TestClass]
    public class StudentControllerTest : BaseControllerTest
    {
        private ISchoolDistrictManager MockLogicManager { get; set; }
        private StudentController Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockLogicManager = MockRepository.GenerateMock<ISchoolDistrictManager>();
            Target = new StudentController(MockLogicManager);
            Target.ControllerContext = new ControllerContext(MockHttpContext, new RouteData(), Target);
            MockHttpContext.Expect(m => m.User).Return(User);
        }

        [TestMethod]
        public void GivenNullLogicManager_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new StudentController(null));
        }

        [TestMethod]
        public void WhenStudentFinderActionIsCalled_ThenAViewResultIsCreated()
        {
            var result = Target.Index() as ViewResult;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void WhenStudentFinderActionIsCalled_ThenViewResultHasViewModel()
        {
            StudentListOptionsModel expected = new StudentListOptionsModel();
            MockLogicManager.Expect(m => m.GenerateListOptionsViewModel(User)).Return(expected);

            var result = Target.Index() as ViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void GivenSearchTermHasSearchResults_WhenAutoCompleteFirstNameActionIsCalled_ThenJsonArrayOfStudentFirstNamesIsReturned()
        {
            var searchTerm = "Mar";
            var expected = new List<string> { "Mark", "Micah" };
            MockLogicManager.Expect(m => m.SearchFirstNames(User, searchTerm)).Return(expected);

            var result = Target.AutocompleteFirstName(searchTerm) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenSearchTermLessThan3Chars_WhenAutoCompleteFirstNameActionIsCalled_ThenReturnedJsonArrayIsNull()
        {
            var searchTerm = "M";

            var result = Target.AutocompleteFirstName(searchTerm) as JsonResult;

            Assert.IsNull(result.Data);
            MockLogicManager.AssertWasNotCalled(m => m.SearchFirstNames(User, searchTerm));
        }

        [TestMethod]
        public void GivenSearchTermHasSearchResults_WhenAutoCompleteLastNameActionIsCalled_ThenJsonArrayOfStudentLastNamesIsReturned()
        {
            var searchTerm = "Ova";
            var expected = new List<string> { "Ovadia" };
            MockLogicManager.Expect(m => m.SearchLastNames(User, searchTerm)).Return(expected);

            var result = Target.AutocompleteLastName(searchTerm) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenSearchTermLessThan3Chars_WhenAutoCompleteLastNameActionIsCalled_ThenReturnedJsonArrayIsNull()
        {
            var searchTerm = "O";

            var result = Target.AutocompleteLastName(searchTerm) as JsonResult;

            Assert.IsNull(result.Data);
            MockLogicManager.AssertWasNotCalled(m => m.SearchLastNames(User, searchTerm));
        }

        [TestMethod]
        public void GivenSearchTermHasSearchResults_WhenAutoCompleteIdActionIsCalled_ThenJsonArrayOfStudentIdentifiersIsReturned()
        {
            var searchTerm = "123";
            var expected = new List<string> { "123" };
            MockLogicManager.Expect(m => m.SearchIdentifiers(User, searchTerm)).Return(expected);

            var result = Target.AutocompleteId(searchTerm) as JsonResult;

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenSearchTermLessThan3Chars_WhenAutoCompleteIdActionIsCalled_ThenReturnedJsonArrayIsNull()
        {
            var searchTerm = "1";

            var result = Target.AutocompleteId(searchTerm) as JsonResult;

            Assert.IsNull(result.Data);
            MockLogicManager.AssertWasNotCalled(m => m.SearchIdentifiers(User, searchTerm));
        }
        
        [TestMethod]
        public void WhenStudentDetailsActionIsCalled_ThenAViewResultIsCreated()
        {
            StudentDetailModel expected = new StudentDetailModel();
            MockLogicManager.Expect(m => m.GenerateStudentDetailViewModel(User, 1)).Return(expected);

            ActionResult result = Target.Details(1);

            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void GivenAStudentId_WhenStudentDetailsActionIsCalled_ThenAViewResultForTheDesiredStudentIsReturned()
        {
            StudentDetailModel expected = new StudentDetailModel();
            MockLogicManager.Expect(m => m.GenerateStudentDetailViewModel(User, 1)).Return(expected);

            var result = Target.Details(1) as ViewResult;

            result.AssertGetViewModel(expected);
        }

        [TestMethod]
        public void WhenDataTableAjaxHandler_ThenReturnViewModelFromLogicManager()
        {
            DataTableRequestModel model = new DataTableRequestModel();
            var expected = new DataTableResultModel();
            MockLogicManager.Expect(m => m.GenerateDataTableResultViewModel(Arg.Is(model), Arg<IClientDataTable<Student>>.Is.NotNull)).Return(expected);
            MockLogicManager.Expect(m => m.FindStudentProperties()).Return(Enumerable.Empty<Property>().ToList());

            var result = Target.DataTableAjaxHandler(model);

            result.AssertGetData(expected);
        }

        [TestMethod]
        public void GivenLogicManagerFindsStudents_WhenGetAllFilteredStudentIds_ThenIdsOfFoundStudentsReturned()
        {
            var expected = new List<int> { 3, 5 };
            MockLogicManager.Expect(m => m.GetFilteredFinderStudentIds(Arg.Is(User), Arg<IClientDataTable<Student>>.Is.NotNull)).Return(expected);
            MockLogicManager.Expect(m => m.FindStudentProperties()).Return(Enumerable.Empty<Property>().ToList());

            var result = Target.AllFilteredStudentIds();

            var actual = result.AssertGetData<IEnumerable<int>>();
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void WhenISelectAllStudentIdsInStudentFinder_ThenIwillGetAllStudentIds()
        {
            var data = new TestData();
            var allStudents = data.Students;
            var expected = allStudents.Where(s => s.School.Name.Contains("Columbus")).Select(s => s.Id).ToList();
            MockLogicManager.Expect(m => m.GetFilteredFinderStudentIds(Arg.Is(User), Arg<IClientDataTable<Student>>.Is.NotNull)).Return(expected);
            MockLogicManager.Expect(m => m.FindStudentProperties()).Return(data.Properties);
            User.Identity.User.UserRoles.Add(new UserRole
            {
                Role = new Role { Name = SecurityRoles.SiteCoordinator },
                Schools = new List<School> { new School { Id = 4, Name = "Columbus High School" } }
            });

            var result = Target.AllFilteredStudentIds();

            var actual = result.AssertGetData<IEnumerable<int>>();
            CollectionAssert.AreEqual(expected, actual.ToList());
        }
    }
}