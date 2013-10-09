using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Collections;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    [TestClass]
    public class SchoolDistrictManagerTest : BaseManagerTest
    {
        private SchoolDistrictManager Target { get; set; }
        private IUserAuditor MockUserAuditor { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            MockUserAuditor = MockRepository.GenerateMock<IUserAuditor>();
            Target = new SchoolDistrictManager(Repositories.MockRepositoryContainer, MockDataTableBinder, MockUserAuditor);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new SchoolDistrictManager(null, MockDataTableBinder, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new SchoolDistrictManager(Repositories.MockRepositoryContainer, null, MockUserAuditor));
        }

        [TestMethod]
        public void GivenNullUserAuditor_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new SchoolDistrictManager(Repositories.MockRepositoryContainer, MockDataTableBinder, null));
        }

        [TestMethod]
        public void GivenNoStudentsWithSchools_WhenGenerateApprovalListViewModel_ThenViewModelSchoolFilterListIsEmpty()
        {
            var target = CreateTargetWithEmptyLookupLists();

            StudentApprovalListOptionsModel actual = target.GenerateApprovalListOptionsViewModel();

            Assert.AreEqual(0, actual.SchoolFilterList.Count());
        }

        [TestMethod]
        public void GivenNoProviders_WhenGenerateApprovalListViewModel_ThenViewModelProviderFilterListIsEmpty()
        {
            var target = CreateTargetWithEmptyLookupLists();

            StudentApprovalListOptionsModel actual = target.GenerateApprovalListOptionsViewModel();

            Assert.AreEqual(0, actual.ProviderFilterList.Count());
        }

        [TestMethod]
        public void GivenStudentsWithSchools_WhenGenerateApprovalListViewModel_ThenViewModelSchoolFilterListContainsSchoolNames()
        {
            var expected = new List<string> { "Bombay Education Center", "Columbus High School", "Evergreen High School", "Wyoming High School" };
            List<Student> students = new TestData().Students;
            Repositories.MockStudentRepository.Expect(m => m.Items).Return(students.AsQueryable());
            Repositories.MockProviderRepository.Expect(m => m.Items).Return(Enumerable.Empty<Provider>().AsQueryable());

            StudentApprovalListOptionsModel actual = Target.GenerateApprovalListOptionsViewModel();

            CollectionAssert.AreEqual(expected, actual.SchoolFilterList.ToList());
        }

        [TestMethod]
        public void GivenProviders_WhenGenerateApprovalListViewModel_ThenViewModelProviderFilterListContainsProviderNames()
        {
            var expected = new List<string> { "Big Brothers, Big Sisters", "Jimbo's Math Shop", "YMCA" };
            List<Provider> providers = new TestData().Providers;
            Repositories.MockProviderRepository.Expect(m => m.Items).Return(providers.AsQueryable());
            Repositories.MockStudentRepository.Expect(m => m.Items).Return(Enumerable.Empty<Student>().AsQueryable());

            StudentApprovalListOptionsModel actual = Target.GenerateApprovalListOptionsViewModel();

            CollectionAssert.AreEqual(expected, actual.ProviderFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateApprovalListViewModel_ThenViewModelHasCountOfStudentsWithAtLeastOneApprovedProvider()
        {
            var testData = new TestData();
            Repositories.MockProviderRepository.Expect(m => m.Items).Return(testData.Providers.AsQueryable());
            Repositories.MockStudentRepository.Expect(m => m.Items).Return(testData.Students.AsQueryable());

            StudentApprovalListOptionsModel actual = Target.GenerateApprovalListOptionsViewModel();

            Assert.AreEqual(4, actual.TotalStudentsWithApproval);
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateAddStudentApprovalViewModel_ThenViewModelReturned()
        {
            AddStudentApprovalModel actual = Target.GenerateAddStudentApprovalViewModel(3);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenInvalidStudentId_WhenGenerateAddStudentApprovalViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateAddStudentApprovalViewModel(100));
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateAddStudentApprovalViewModel_ThenViewModelContainsStudentId()
        {
            var expectedStudentId = 3;
            Student toFind = Repositories.MockStudentRepository.Items.Where(s => s.Id == expectedStudentId).Single();

            AddStudentApprovalModel result = Target.GenerateAddStudentApprovalViewModel(expectedStudentId);

            int actual = result.StudentId;
            Assert.IsNotNull(actual);
            Assert.AreEqual(expectedStudentId, actual);
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateAddStudentApprovalViewModel_ThenViewModelContainsListOfProvidersNotAlreadyAssociated()
        {
            Student toFind = Repositories.MockStudentRepository.Items.Where(s => s.Id == 3).Single();
            toFind.ApprovedProviders.Add(Repositories.MockProviderRepository.Items.Where(p => p.Id == 2).Single());
            IEnumerable<Provider> expectedProviders = Repositories.MockProviderRepository.Items.Where(p => p.Id != 2);

            AddStudentApprovalModel result = Target.GenerateAddStudentApprovalViewModel(3);

            MultiSelectList actual = result.Providers;
            Assert.IsNotNull(actual);
            Assert.AreEqual("Id", actual.DataValueField);
            Assert.AreEqual("Name", actual.DataTextField);
            CollectionAssert.AreEqual(expectedProviders.ToList(), actual.Items.Cast<Provider>().ToList());
        }

        [TestMethod]
        public void GivenNullViewModel_WhenAddProviders_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.AddProviders(null));
        }

        [TestMethod]
        public void GivenViewModelWithInvalidStudentId_WhenAddProviders_ThenThrowException()
        {
            AddStudentApprovalModel viewModel = new AddStudentApprovalModel { StudentId = 100 };

            Target.ExpectException<EntityNotFoundException>(() => Target.AddProviders(viewModel));
        }

        [TestMethod]
        public void GivenViewModelWithValidStudentId_AndSingleSelectedProviderIsInvalid_WhenAddProviders_ThenThrowException()
        {
            AddStudentApprovalModel viewModel = new AddStudentApprovalModel { StudentId = 1, ProvidersToAdd = new[] { 100 } };

            Target.ExpectException<EntityNotFoundException>(() => Target.AddProviders(viewModel));
        }

        [TestMethod]
        public void GivenViewModelWithValidStudentId_AndSecondOfTwoSelectedProvidersIsInvalid_WhenAddProviders_ThenThrowException()
        {
            AddStudentApprovalModel viewModel = new AddStudentApprovalModel { StudentId = 1, ProvidersToAdd = new[] { 1, 100 } };

            Target.ExpectException<EntityNotFoundException>(() => Target.AddProviders(viewModel));
        }

        [TestMethod]
        public void GivenViewModelWithValidStudentId_AndValidListOfProviders_WhenAddProviders_ThenAddLinkCalledBetweenStudentAndAllSelectedProviders()
        {
            var studentId = 1;
            int[] providerIds = new[] { 1, 2 };
            Student expectedStudent = Repositories.MockStudentRepository.Items.Where(s => s.Id == studentId).Single();
            IEnumerable<Provider> expectedProviders = Repositories.MockProviderRepository.Items.Where(p => providerIds.Contains(p.Id));
            var viewModel = new AddStudentApprovalModel { StudentId = studentId, ProvidersToAdd = providerIds };

            Target.AddProviders(viewModel);

            foreach (Provider expectedProvider in expectedProviders)
            {
                Repositories.MockStudentRepository.AssertWasCalled(m => m.AddLink(expectedStudent, expectedProvider));
            }
        }

        [TestMethod]
        public void GivenViewModelWithValidStudentId_AndValidListOfProviders_WhenAddProviders_ThenSaveChanges()
        {
            var viewModel = new AddStudentApprovalModel { StudentId = 1, ProvidersToAdd = new[] { 1, 2 } };

            Target.AddProviders(viewModel);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidStudentId_WhenGenerateRemoveProviderViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateRemoveProviderViewModel(100, 2));
        }

        [TestMethod]
        public void GivenValidStudentId_AndInvalidProviderId_WhenGenerateRemoveProviderViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateRemoveProviderViewModel(4, 100));
        }

        [TestMethod]
        public void GivenValidStudentId_AndValidProviderId_AndProviderIsNotApproveByStudent_WhenGenerateRemoveProviderViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateRemoveProviderViewModel(1, 1));
        }

        [TestMethod]
        public void GivenValidStudentId_AndValidProviderId_AndProviderIsApproveByStudent_WhenGenerateRemoveProviderViewModel_ThenViewModelIsGenerated()
        {
            RemoveApprovedProviderModel actual = Target.GenerateRemoveProviderViewModel(2, 1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidStudentId_AndValidProviderId_AndProviderIsApproveByStudent_WhenGenerateRemoveProviderViewModel_ThenViewModelContainsEntityState()
        {
            var expected = new RemoveApprovedProviderModel { ProviderId = 1, StudentId = 2, ProviderName = "YMCA", StudentName = "Jain, Nidhi Quizzno" };

            RemoveApprovedProviderModel actual = Target.GenerateRemoveProviderViewModel(2, 1);

            Assert.AreEqual(expected.ProviderId, actual.ProviderId);
            Assert.AreEqual(expected.ProviderName, actual.ProviderName);
            Assert.AreEqual(expected.StudentId, actual.StudentId);
            Assert.AreEqual(expected.StudentName, actual.StudentName);
        }

        [TestMethod]
        public void GivenInvalidStudentId_WhenRemoveProvider_ThenThrowEntityNotFoundException()
        {
            var viewModel = new RemoveApprovedProviderModel { ProviderId = 2, StudentId = 100 };

            Target.ExpectException<EntityNotFoundException>(() => Target.RemoveProvider(viewModel));
        }

        [TestMethod]
        public void GivenValidStudentId_AndInvalidProviderId_WhenRemoveProvider_ThenThrowEntityNotFoundException()
        {
            var viewModel = new RemoveApprovedProviderModel { ProviderId = 100, StudentId = 4 };

            Target.ExpectException<EntityNotFoundException>(() => Target.RemoveProvider(viewModel));
        }

        [TestMethod]
        public void GivenValidStudentId_AndValidProviderId_AndProviderIsNotApproveByStudent_WhenRemoveProvider_ThenThrowEntityNotFoundException()
        {
            var viewModel = new RemoveApprovedProviderModel { ProviderId = 1, StudentId = 1 };

            Target.ExpectException<EntityNotFoundException>(() => Target.RemoveProvider(viewModel));
        }

        [TestMethod]
        public void GivenValidStudentId_AndValidProviderId_AndProviderIsApproveByStudent_WhenRemoveProvider_ThenProviderApprovalRemoved_AndSaved()
        {
            var viewModel = new RemoveApprovedProviderModel { ProviderId = 2, StudentId = 4 };
            Student expectedStudent = Repositories.MockStudentRepository.Items.Where(s => s.Id == viewModel.StudentId).Single();
            Provider expectedProvider = Repositories.MockProviderRepository.Items.Where(p => p.Id == viewModel.ProviderId).Single();

            Target.RemoveProvider(viewModel);

            Repositories.MockStudentRepository.AssertWasCalled(m => m.DeleteLink(expectedStudent, expectedProvider));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenGenerateRemoveAllProvidersBySchoolViewModel_ThenViewModelIsReturned()
        {
            RemoveApprovedProvidersBySchoolModel actual = Target.GenerateRemoveProvidersBySchoolViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateRemoveAllProvidersBySchoolViewModel_ThenViewModelSchoolListPopulated()
        {
            List<School> expectedSchools = Repositories.MockStudentRepository.Items.Select(s => s.School).Distinct().ToList();

            RemoveApprovedProvidersBySchoolModel actual = Target.GenerateRemoveProvidersBySchoolViewModel();

            CollectionAssert.AreEquivalent(expectedSchools, actual.Schools.Items.Cast<School>().ToList());
            Assert.AreEqual("Id", actual.Schools.DataValueField);
            Assert.AreEqual("Name", actual.Schools.DataTextField);
        }

        [TestMethod]
        public void WhenRemoveAllProviders_ThenRepositoryResetsApprovals()
        {
            Target.RemoveAllProviders();

            Repositories.MockStudentRepository.AssertWasCalled(m => m.ResetApprovals());
        }

        [TestMethod]
        public void WhenRemoveAllProviders_ThenChangesSaved()
        {
            Target.RemoveAllProviders();

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenSchoolIds_WhenRemoveAllProviders_ThenRepositoryResetsApprovals()
        {
            IEnumerable<int> expected = new[] { 39, 328 };

            Target.RemoveAllProviders(expected);

            Repositories.MockStudentRepository.AssertWasCalled(m => m.ResetApprovals(expected));
        }

        [TestMethod]
        public void GivenSchoolIds_WhenRemoveAllProviders_ThenChangesSaved()
        {
            IEnumerable<int> expected = new[] { 39, 328 };

            Target.RemoveAllProviders(expected);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidStudentId_WhenSetStudentOptOutState_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.SetStudentOptOutState(100, true));
        }

        [TestMethod]
        public void GivenValidStudentId_WhenSetStudentOptOutState_ThenStudentHasParentalOptOutStateChanged()
        {
            Student expected = Repositories.MockStudentRepository.Items.Where(s => s.Id == 1).Single();
            expected.HasParentalOptOut = true;

            Target.SetStudentOptOutState(1, false);

            Assert.IsFalse(expected.HasParentalOptOut);
        }

        [TestMethod]
        public void GivenValidStudentId_WhenSetStudentOptOutState_ThenStudentUpdated_AndSaved()
        {
            Student expected = Repositories.MockStudentRepository.Items.Where(s => s.Id == 2).Single();

            Target.SetStudentOptOutState(2, true);

            Repositories.MockStudentRepository.AssertWasCalled(m => m.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenViewModelHasFilterLists()
        {
            Func<Student, string> orderingFunction = (s => s.School.Name);
            var expected = new StudentListOptionsModel
            {
                GradeFilterList = Repositories.MockStudentRepository.Items.Select(i => i.Grade).Distinct(),
                SchoolFilterList = Repositories.MockStudentRepository.Items.OrderBy(orderingFunction, new NaturalSortComparer<string>()).Select(s => s.School.Name).Distinct(),
                PriorityFilterList = Repositories.MockPriorityRepository.Items.Select(p => p.Name),
                ServiceTypeFilterList = Repositories.MockServiceTypeRepository.Items.Where(s => s.IsActive).Select(t => t.Name),
                SubjectFilterList = Repositories.MockSubjectRepository.Items.Select(s => s.Name)
            };

            StudentListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEqual(expected.GradeFilterList.ToList(), actual.GradeFilterList.ToList());
            CollectionAssert.AreEqual(expected.SchoolFilterList.ToList(), actual.SchoolFilterList.ToList());
            CollectionAssert.AreEqual(expected.PriorityFilterList.ToList(), actual.PriorityFilterList.ToList());
            CollectionAssert.AreEqual(expected.ServiceTypeFilterList.ToList(), actual.ServiceTypeFilterList.ToList());
            CollectionAssert.AreEqual(expected.SubjectFilterList.ToList(), actual.SubjectFilterList.ToList());
        }

        [TestMethod]
        public void GivenNullUser_WhenGenerateListOptionsViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GenerateListOptionsViewModel(null));
        }

        [TestMethod]
        public void GivenUserIsProvider_WhenGenerateListOptionsViewModel_ThenViewModelIsProviderTrue()
        {
            User.Identity.User.UserRoles = Data.UserRoles.Where(ur => ur.Role == Data.Roles.Where(r => r.Name.Equals(SecurityRoles.Provider)).SingleOrDefault()).ToList();

            var actual = Target.GenerateListOptionsViewModel(User);

            Assert.IsTrue(actual.IsProvider);
        }

        [TestMethod]
        public void GivenUserIsSiteCoordinator_AndUserIsProvider_WhenGenerateListOptionsViewModel_ThenViewModelIsProviderFalse()
        {
            User.Identity.User.UserRoles = new List<UserRole> { new UserRole { Role = new Role { Name = SecurityRoles.Provider } }, new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator } } };

            var actual = Target.GenerateListOptionsViewModel(User);

            Assert.IsFalse(actual.IsProvider);
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenViewModelReqestStatusFilterListPopulated()
        {
            var expected = new List<string> { Statuses.Open, Statuses.Fulfilled, Statuses.Rejected };

            var actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(expected, actual.RequestStatusFilterList.ToList());
        }

        [TestMethod]
        public void GivenSearchTerm_WhenSearchFirstNames_ThenListOfStudentFirstNamesIsReturned()
        {
            var searchTerm = "N";
            var expected = new List<string> { "Nick", "Nidhi" };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Data.Students.AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenSearchTermWithNoCredentials_WhenSearchFirstNames_ThenAnEmptyListOfStudentFirstNamesIsReturned()
        {
            var searchTerm = "N";
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Enumerable.Empty<Student>().AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            Assert.IsFalse(actual.ToList().Any());
        }

        [TestMethod]
        public void GivenDifferentSearchTerm_WhenSearchFirstNames_ThenListOfStudentFirstNamesIsReturned()
        {
            var searchTerm = "M";
            var expected = new List<string> { "Mark", "Micah" };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Data.Students.AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenSearchWillReturnMoreThan10Results_WhenSearchFirstNames_ThenOnly10Returned()
        {
            var searchTerm = "Mar";
            List<Student> students = new List<Student> 
            {
                new Student { FirstName = "Mark" , SchoolId = 1},
                new Student { FirstName = "Marc" , SchoolId = 1},
                new Student { FirstName = "Marb" , SchoolId = 1},
                new Student { FirstName = "Marg" , SchoolId = 1},
                new Student { FirstName = "Marp" , SchoolId = 1},
                new Student { FirstName = "Marq" , SchoolId = 1},
                new Student { FirstName = "Mare" , SchoolId = 1},
                new Student { FirstName = "Marr" , SchoolId = 1},
                new Student { FirstName = "Mart" , SchoolId = 1},
                new Student { FirstName = "Mary" , SchoolId = 1},
                new Student { FirstName = "Mari" , SchoolId = 1},
            };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(students.AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            Assert.AreEqual(10, actual.Count());
        }

        [TestMethod]
        public void GivenDifferentSearchTermWithNoCredentials_WhenSearchFirstNames_ThenAnEmptyListOfStudentFirstNamesIsReturned()
        {
            var searchTerm = "M";
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Enumerable.Empty<Student>().AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            Assert.IsFalse(actual.ToList().Any());
        }

        [TestMethod]
        public void GivenSearchWillReturnMoreThan10Results_WhenSearchFirstNames_ThenOnlyTop10ResultsReturned()
        {
            var searchTerm = "M";
            List<Student> students = new List<Student> { 
                new Student { FirstName = "Mark", SchoolId = 1 },
                new Student { FirstName = "Mike", SchoolId = 1 },
                new Student { FirstName = "Michael", SchoolId = 1 },
                new Student { FirstName = "Micah", SchoolId = 1 },
                new Student { FirstName = "Mickey", SchoolId = 1 },
                new Student { FirstName = "Mikey", SchoolId = 1 },
                new Student { FirstName = "Marv", SchoolId = 1 },
                new Student { FirstName = "Marky", SchoolId = 1 },
                new Student { FirstName = "Marky Mark", SchoolId = 1 },
                new Student { FirstName = "Man", SchoolId = 1 },
                new Student { FirstName = "Matt", SchoolId = 1 }
            };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(students.AsQueryable());

            var actual = Target.SearchFirstNames(User, searchTerm);

            Assert.AreEqual(10, actual.Count());
        }

        [TestMethod]
        public void GivenSearchTerm_WhenSearchLastNames_ThenListOfStudentLastNamesIsReturned()
        {
            var searchTerm = "O";
            var expected = new List<string> { "Ovadia" };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Data.Students.AsQueryable());

            var actual = Target.SearchLastNames(User, searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenDifferentSearchTerm_WhenSearchLastNames_ThenListOfStudentLastNamesIsReturned()
        {
            var searchTerm = "Glecker";
            var expected = new List<string> { "Glecker" };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Data.Students.AsQueryable());

            var actual = Target.SearchLastNames(User, searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenDifferentSearchTermWithNoCredentials_WhenSearchLastNames_ThenAnEmptyListOfStudentLastNamesIsReturned()
        {
            var searchTerm = "Glecker";
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Enumerable.Empty<Student>().AsQueryable());

            var actual = Target.SearchLastNames(User, searchTerm);

            Assert.IsFalse(actual.ToList().Any());
        }

        [TestMethod]
        public void GivenSearchWillReturnMoreThan10Results_WhenSearchLastNames_ThenOnlyTop10ResultsReturned()
        {
            var searchTerm = "M";
            List<Student> students = new List<Student> { 
                new Student { LastName = "Mark", SchoolId = 1 },
                new Student { LastName = "Mike", SchoolId = 1 },
                new Student { LastName = "Michael", SchoolId = 1 },
                new Student { LastName = "Micah", SchoolId = 1 },
                new Student { LastName = "Mickey", SchoolId = 1 },
                new Student { LastName = "Mikey", SchoolId = 1 },
                new Student { LastName = "Marv", SchoolId = 1 },
                new Student { LastName = "Marky", SchoolId = 1 },
                new Student { LastName = "Marky Mark", SchoolId = 1 },
                new Student { LastName = "Man", SchoolId = 1 },
                new Student { LastName = "Matt", SchoolId = 1 }
            };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(students.AsQueryable());

            var actual = Target.SearchLastNames(User, searchTerm);

            Assert.AreEqual(10, actual.Count());
        }

        [TestMethod]
        public void GivenRepositoryGetsAllStudentsAllowedStudentList_WhenSearchIdentifiers_ThenListOfStudentIDsIsReturned()
        {
            var searchTerm = "10";
            var expected = new List<string> { "10" };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(Data.Students.AsQueryable());

            var actual = Target.SearchIdentifiers(User, searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenSearchWillReturnMoreThan10Results_WhenSearchIdentifiers_ThenOnlyTop10ResultsReturned()
        {
            var searchTerm = "123";
            List<Student> students = new List<Student> { 
                new Student { StudentSISId = "123", SchoolId = 1 },
                new Student { StudentSISId = "1231", SchoolId = 1 },
                new Student { StudentSISId = "1232", SchoolId = 1 },
                new Student { StudentSISId = "1233", SchoolId = 1 },
                new Student { StudentSISId = "1234", SchoolId = 1 },
                new Student { StudentSISId = "1235", SchoolId = 1 },
                new Student { StudentSISId = "1236", SchoolId = 1 },
                new Student { StudentSISId = "1237", SchoolId = 1 },
                new Student { StudentSISId = "1238", SchoolId = 1 },
                new Student { StudentSISId = "1239", SchoolId = 1 },
                new Student { StudentSISId = "1230", SchoolId = 1 }
            };
            Repositories.MockStudentRepository.Expect(m => m.GetAllowedList(User)).Return(students.AsQueryable());

            var actual = Target.SearchIdentifiers(User, searchTerm);

            Assert.AreEqual(10, actual.Count());
        }

        [TestMethod]
        public void GivenValidStudentId_AndPermissionGrantsAccess_WhenGenerateStudentDetailViewModel_ThenResultIsNotNull()
        {
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            var result = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidStudentId_AndPermissionGrantsAccess_WhenGenerateStudentDetailViewModel_ThenCorrespondingStudentReturned()
        {
            var expectedState = Data.Students[0];
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            var actual = Target.GenerateStudentDetailViewModel(User, 1);

            AssertViewModel(expectedState, actual);
        }

        [TestMethod]
        public void GivenInvalidStudentId_WhenGenerateStudentDetailViewModel_ThenAEntityNotFoundExceptionIsThrown()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateStudentDetailViewModel(User, 100));
        }

        [TestMethod]
        public void GivenValidStudentId__WhenGenerateStudentDetailViewModel_ThenAttemptGrantAccess()
        {
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(permission);
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            Target.GenerateStudentDetailViewModel(User, 1);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidStudentId_AndPermissionCustomFieldOnlyTrue_WhenGenerateStudentDetailViewModel_ThenViewModelContainsOnlyCustomFieldValuesByUser_AndViewModelOnlyUploadedCustomFieldSetTrue()
        {
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.CustomFieldOnly).Return(true);
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(permission);
            User.Identity.User.Id = 1;
            var expected = Data.CustomFieldValues.Where(c => c.CustomDataOrigin.CreatingUserId == 1 && c.StudentId == 1);

            var actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.IsTrue(actual.OnlyUploadedCustomField);
            Assert.IsTrue(actual.CustomData.Count() > 0);
            CollectionAssert.AreEquivalent(expected.Select(e => e.CustomField.Name).ToList(), actual.CustomData.Select(c => c.FieldName).ToList());
        }

        [TestMethod]
        public void GivenValidStudentId_AndPermissionCustomFieldOnlyTrue_WhenGenerateStudentDetailViewModel_ThenViewModelDoesNotContainPrivateStudentData()
        {
            IViewStudentDetailPermission permission = MockRepository.GenerateMock<IViewStudentDetailPermission>();
            permission.Expect(p => p.CustomFieldOnly).Return(true);
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(permission);
            User.Identity.User.Id = 1;

            var actual = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.IsFalse(actual.DateOfBirth.HasValue);
            Assert.IsNull(actual.Parents);
            Assert.IsTrue(actual.ServiceRequests.Count() == 0);
            Assert.IsTrue(actual.StudentAssignedOfferings.Count() == 0);
            Assert.IsTrue(actual.Classes.Count() == 0);
        }

        [TestMethod]
        public void GivenValidStudentId_AndPermissionCustomFieldOnlyFalse_WhenGenerateStudentDetailViewModel_ThenViewModelCustomFieldOnlyFalse()
        {
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }

            var result = Target.GenerateStudentDetailViewModel(User, 1);

            Assert.IsFalse(result.OnlyUploadedCustomField);
        }

        [TestMethod]
        public void GivenValidStudentId_AndCustomFieldPermissionDoesntAllowAccessToSomeFields_WhenGenerateStudentDetailViewModel_ThenModelHasExpectedCustomFields()
        {
            var expected = Data.Students.First().CustomFieldValues.Where(c => c.Id != 1);
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", Data.Students.Single(s => s.Id == 1).CustomFieldValues.First().CustomField)).Return(permission);
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                if (field.Id != 1)
                {
                    PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
                }
            }

            var result = Target.GenerateStudentDetailViewModel(User, 1);

            CollectionAssert.AreEqual(expected.Select(e => e.Value).ToList(), result.CustomData.Select(c => c.Value).ToList());
        }

        [TestMethod]
        public void GivenValidStudentId_AndCustomFieldPermissionDoesntAllowAccessToSomeFields_AndUserIsAuthorOfThoseFields_WhenGenerateStudentDetailViewModel_ThenModelHasExpectedCustomFields()
        {
            var expected = Data.Students.First().CustomFieldValues;
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            permission.Expect(p => p.GrantAccess(User)).Throw(new EntityAccessUnauthorizedException());
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", Data.Students.Single(s => s.Id == 1).CustomFieldValues.First().CustomField)).Return(permission);
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                if (field.Id != 1)
                {
                    PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
                }
            }
            User.Identity.User.Id = 1;

            var result = Target.GenerateStudentDetailViewModel(User, 1);

            CollectionAssert.AreEquivalent(expected.Select(e => e.Value).ToList(), result.CustomData.Select(c => c.Value).ToList());
        }

        [TestMethod]
        public void GivenValidStudentId_WhenGenerateStudentDetailViewModel_ThenPrivateHealthDataViewRepositorySaves()
        {
            PermissionFactory.Current.Expect(m => m.Create("ViewStudentDetail", Data.Students.Single(s => s.Id == 1))).Return(MockRepository.GenerateMock<IViewStudentDetailPermission>());
            foreach (var field in Data.Students.Single(s => s.Id == 1).CustomFieldValues.Select(c => c.CustomField))
            {
                PermissionFactory.Current.Expect(m => m.Create("ViewStudentCustomFieldData", field)).Return(MockRepository.GenerateMock<IPermission>());
            }
            PrivateHealthDataViewEvent privateHealthDataViewEvent = new PrivateHealthDataViewEvent{ Id = 1 };
            MockUserAuditor.Expect(m => m.CreatePrivateHealthInfoViewEvent(User.Identity.User, Data.Students.Single(s => s.Id == 1).CustomFieldValues.Where(c => c.CustomField is PrivateHealthField).ToList())).Return(privateHealthDataViewEvent);

            var result = Target.GenerateStudentDetailViewModel(User, 1);

            Repositories.MockPrivateHealthDataViewEventRepository.AssertWasCalled(m => m.Add(privateHealthDataViewEvent));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<Student> dataTable = MockRepository.GenerateMock<IClientDataTable<Student>>();
            MockDataTableBinder.Expect(m => m.Bind(Repositories.MockStudentRepository.Items, dataTable, requestModel)).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenIGetFilteredFinderStudentIds_ThenAllAreReturned()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });
            var expected = Repositories.MockStudentRepository.Items.Select(m => m.Id).ToList();
            var dataTable = MockRepository.GenerateMock<IClientDataTable<Student>>();
            dataTable.Expect(m => m.ApplyFilters(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items);
            dataTable.Expect(m => m.ApplySort(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items.OrderBy(s => s.Id));

            var actual = Target.GetFilteredFinderStudentIds(User, dataTable);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenUserIsAnUnassignedSiteCoordinator_WhenIGetFilteredFinderStudentIds_ThenAnEmptyListIsReturned()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator } });
            var expected = new List<int>();
            var dataTable = MockRepository.GenerateMock<IClientDataTable<Student>>();
            dataTable.Expect(m => m.ApplyFilters(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items);
            dataTable.Expect(m => m.ApplySort(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items.OrderBy(s => s.Id));

            var actual = Target.GetFilteredFinderStudentIds(User, dataTable);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenUserIsAnAssignedSiteCoordinator_WhenIGetFilteredFinderStudentIds_ThenAllAreReturned()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.SiteCoordinator }, Schools = Data.Schools });
            var expected = Repositories.MockStudentRepository.Items.Select(m => m.Id).ToList();
            var dataTable = MockRepository.GenerateMock<IClientDataTable<Student>>();
            dataTable.Expect(m => m.ApplyFilters(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items);
            dataTable.Expect(m => m.ApplySort(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items.OrderBy(s => s.Id));

            var actual = Target.GetFilteredFinderStudentIds(User, dataTable);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenUserIsAProvider_WhenIGetFilteredFinderStudentIds_ThenAllAreReturned()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.Provider } });
            var expected = Repositories.MockStudentRepository.Items.Select(m => m.Id).ToList();
            var dataTable = MockRepository.GenerateMock<IClientDataTable<Student>>();
            dataTable.Expect(m => m.ApplyFilters(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items);
            dataTable.Expect(m => m.ApplySort(Repositories.MockStudentRepository.Items)).Return(Repositories.MockStudentRepository.Items.OrderBy(s => s.Id));

            var actual = Target.GetFilteredFinderStudentIds(User, dataTable);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenNoStudentsWithApprovedProviders_WhenCountStudentsWithApprovedProviders_ThenReturnZero()
        {
            Data.Students.ForEach(a => a.ApprovedProviders.Clear());

            int actual = Target.CountStudentsWithApprovedProviders();

            Assert.AreEqual(0, actual);
        }

        [TestMethod]
        public void GivenOneStudentsWithApprovedProviders_WhenCountStudentsWithApprovedProviders_ThenReturnOne()
        {
            Data.Students.ForEach(a => a.ApprovedProviders.Clear());
            Data.Students[2].ApprovedProviders.Add(new Provider());

            int actual = Target.CountStudentsWithApprovedProviders();

            Assert.AreEqual(1, actual);
        }

        [TestMethod]
        public void GivenAllStudentsWithApprovedProviders_WhenCountStudentsWithApprovedProviders_ThenReturnStudentCount()
        {
            Data.Students.ForEach(a => a.ApprovedProviders.Add(new Provider()));

            int actual = Target.CountStudentsWithApprovedProviders();

            Assert.AreEqual(Data.Students.Count, actual);
        }

        [TestMethod]
        public void WhenGenerateSchoolSelectorViewModel_ThenViewModelReturned()
        {
            SchoolSelectorModel actual = Target.GenerateSchoolSelectorViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateSchoolSelectorViewModel_ThenViewModelContainsSchools()
        {
            List<School> expected = Repositories.MockSchoolRepository.Items.ToList();

            SchoolSelectorModel actual = Target.GenerateSchoolSelectorViewModel();

            CollectionAssert.AreEqual(expected, actual.Schools.Items.Cast<School>().ToList());
        }

        [TestMethod]
        public void WhenGenerateSchoolSelectorViewModel_ThenSchoolListPropertiesCorrect()
        {
            SchoolSelectorModel actual = Target.GenerateSchoolSelectorViewModel();

            Assert.AreEqual("Id", actual.Schools.DataValueField);
            Assert.AreEqual("Name", actual.Schools.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateGradeSelectorViewModel_ThenViewModelReturned()
        {
            GradeSelectorModel actual = Target.GenerateGradeSelectorViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateGradeSelectorViewModel_ThenViewModelContainsSchools()
        {
            List<int> expected = Repositories.MockStudentRepository.Items.Select(s => s.Grade).Distinct().ToList();

            GradeSelectorModel actual = Target.GenerateGradeSelectorViewModel();

            CollectionAssert.AreEqual(expected, actual.Grades.Items.Cast<int>().ToList());
        }

        private void MockRequestParameter(string paramName, string paramValue)
        {
            if (paramValue != null)
            {
                MockHttpContext.Request.Expect(m => m[paramName]).Return(paramValue);
            }
        }

        private void PrepareDataTableRequestParameters(string sortColumn, string sortDirection, string firstName, string lastName, string id, string schools, string grades)
        {
            MockHttpContext.Request.Expect(m => m["iSortCol_0"]).Return(sortColumn); //sorting by ID
            MockHttpContext.Request.Expect(m => m["sSortDir_0"]).Return(sortDirection);
            MockRequestParameter("firstName", firstName);
            MockRequestParameter("lastName", lastName);
            MockRequestParameter("ID", id);
            MockRequestParameter("schools", schools);
            MockRequestParameter("grades", grades);
        }

        private static void AssertDataTableResultModel(DataTableResultModel expected, DataTableResultModel actual)
        {
            Assert.AreEqual(expected.sEcho, actual.sEcho);
            Assert.AreEqual(expected.iTotalDisplayRecords, actual.iTotalDisplayRecords);
            Assert.AreEqual(expected.iTotalRecords, actual.iTotalRecords);
            actual.aaData.Cast<string[]>().ToList().AssertItemsEqual(expected.aaData.Cast<string[]>().ToList());
        }

        private static SchoolDistrictManager CreateTargetWithEmptyLookupLists()
        {
            IRepositoryContainer mockRepositoryContainer = MockRepository.GenerateMock<IRepositoryContainer>();
            IStudentRepository mockStudentRepository = MockRepository.GenerateMock<IStudentRepository>();
            IProviderRepository mockProviderRepository = MockRepository.GenerateMock<IProviderRepository>();
            IDataTableBinder mockDataTableBinder = MockRepository.GenerateMock<IDataTableBinder>();
            IUserAuditor mockUserAuditor = MockRepository.GenerateMock<IUserAuditor>();
            mockStudentRepository.Expect(m => m.Items).Return(Enumerable.Empty<Student>().AsQueryable());
            mockProviderRepository.Expect(m => m.Items).Return(Enumerable.Empty<Provider>().AsQueryable());
            mockRepositoryContainer.Expect(m => m.Obtain<IProviderRepository>()).Return(mockProviderRepository);
            mockRepositoryContainer.Expect(m => m.Obtain<IStudentRepository>()).Return(mockStudentRepository);
            return new SchoolDistrictManager(mockRepositoryContainer, mockDataTableBinder, mockUserAuditor);
        }

        private static void AssertViewModel(Student expectedState, StudentDetailModel actual)
        {
            Assert.AreEqual(expectedState.Classes, actual.Classes);
            Assert.AreEqual(expectedState.DateOfBirth, actual.DateOfBirth);
            Assert.AreEqual(expectedState.Grade, actual.Grade);
            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedState.FullName, actual.Name);
            Assert.AreEqual(expectedState.Parents, actual.Parents);
            Assert.AreEqual(expectedState.School.Name, actual.SchoolName);
            Assert.AreEqual(expectedState.ServiceRequests, actual.ServiceRequests);
            Assert.AreEqual(expectedState.StudentSISId, actual.SISId);
            CollectionAssert.AreEqual(expectedState.StudentAssignedOfferings.ToList(), actual.StudentAssignedOfferings.ToList());
        }
    }
}
