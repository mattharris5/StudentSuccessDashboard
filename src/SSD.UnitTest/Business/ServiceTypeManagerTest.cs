using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ServiceTypeManagerTest : BaseManagerTest
    {
        private ServiceTypeManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceTypeManager(Repositories.MockRepositoryContainer, MockDataTableBinder);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceTypeManager(null, MockDataTableBinder));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceTypeManager(Repositories.MockRepositoryContainer, null));
        }

        [TestMethod]
        public void WhenSearchNames_ThenListOfServiceTypeNamesIsReturned()
        {
            var searchTerm = "p";
            var expected = new List<string> { "Provide College Access" };

            var actual = Target.SearchNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenSearchTermWithNoMatches_WhenSearchNames_ThenAnEmptyListIsReturned()
        {
            var searchTerm = "z";
            var expected = new List<string>();

            var actual = Target.SearchNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GiveNullUser_WhenGenerateListOptionsViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GenerateListOptionsViewModel(null));
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenReturnViewModelWithAListOfCategories()
        {
            ServiceTypeListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEqual(Data.Categories.Select(c => c.Name).ToList(), actual.CategoryFilterList.ToList());
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGenerateListOptionsViewModel_ThenViewModelAllowModifyingTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            ServiceTypeListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            Assert.IsTrue(actual.AllowModifying);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenGenerateListOptionsViewModel_ThenViewModelAllowModifyingFalse()
        {
            ServiceTypeListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            Assert.IsFalse(actual.AllowModifying);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAPartialViewResultIsReturned()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsAdministrator);
        }

        [TestMethod]
        public void GivenUserIsDataAdmin_WhenGenerateCreateViewModel_ThenViewModelIsAdministratorTrue()
        {
            User.Identity.User.UserRoles.Add(new UserRole { Role = new Role { Name = SecurityRoles.DataAdmin } });

            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsAdministrator);
        }

        [TestMethod]
        public void GivenUserIsNotDataAdmin_WhenGenerateCreateViewModel_ThenViewModelIsAdministratorFalse()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.IsNotNull(actual);
            Assert.IsFalse(actual.IsAdministrator);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAPartialViewResultWithAPopulatedCategoryListIsReturned()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.IsNotNull(actual.Categories);
            CollectionAssert.AreEqual(Data.Categories, actual.Categories.Items.Cast<Category>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelCategoryListColumnsAreCorrect()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.AreEqual("Id", actual.Categories.DataValueField);
            Assert.AreEqual("Name", actual.Categories.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenAPartialViewResultWithAPopulatedProgramListIsReturned()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.IsNotNull(actual.Programs);
            CollectionAssert.AreEqual(Data.Programs.Where(p => p.IsActive).ToList(), actual.Programs.Items.Cast<Program>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelProgramListColumnsAreCorrect()
        {
            ServiceTypeModel actual = Target.GenerateCreateViewModel(User);

            Assert.AreEqual("Id", actual.Programs.DataValueField);
            Assert.AreEqual("Name", actual.Programs.DataTextField);
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_WhenValidateForDuplicate_ThenThrowValidationException_AndExceptionMemberNamesIncludesName()
        {
            var model = new ServiceTypeModel { Name = "Mentoring" };

            ValidationException actual = Target.ExpectException<ValidationException>(() => Target.ValidateForDuplicate(model));

            CollectionAssert.Contains(actual.ValidationResult.MemberNames.ToList(), "Name");
        }

        [TestMethod]
        public void GivenViewModelHasNewName_WhenValidateForDuplicate_ThenSucceed()
        {
            var model = new ServiceTypeModel { Name = "Mentoring 2" };

            Target.ValidateForDuplicate(model);
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_AndServiceTypeWithNameIsInactive_WhenValidateForDuplicate_ThenSucceed()
        {
            var model = new ServiceTypeModel { Name = Data.ServiceTypes[5].Name };

            Target.ValidateForDuplicate(model);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenSaveCalled()
        {
            ServiceTypeModel viewModel = new ServiceTypeModel();

            Target.Create(viewModel);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenServiceTypeIsActive()
        {
            ServiceTypeModel viewModel = new ServiceTypeModel();

            Target.Create(viewModel);

            Repositories.MockServiceTypeRepository.AssertWasCalled(m => m.Add(Arg<ServiceType>.Matches(s => s.IsActive == true)));
        }

        [TestMethod]
        public void GivenServiceTypeAlreadyExistsWithName_AndIsInactive_WhenCreate_ThenReactivateServiceType()
        {
            ServiceType inactiveType = Data.ServiceTypes[5];
            ServiceTypeModel viewModel = new ServiceTypeModel();
            viewModel.CopyFrom(inactiveType);

            Target.Create(viewModel);

            Assert.IsTrue(inactiveType.IsActive);
        }

        [TestMethod]
        public void GivenInvalidServiceTypeId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 147));
        }

        [TestMethod]
        public void GivenInactiveServiceTypeId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 6));
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenViewModelIsCreated()
        {
            ServiceTypeModel actual = Target.GenerateEditViewModel(User, 1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenViewModelContainsCategoryList()
        {
            ServiceTypeModel actual = Target.GenerateEditViewModel(User, 1);

            CollectionAssert.AreEqual(Data.Categories, actual.Categories.Items.Cast<Category>().ToList());
        }

        [TestMethod]
        public void WhenGenerateEditViewModel_ThenViewModelContainsProgramList()
        {
            ServiceTypeModel actual = Target.GenerateEditViewModel(User, 1);

            CollectionAssert.AreEqual(Data.Programs.Where(p => p.IsActive).ToList(), actual.Programs.Items.Cast<Program>().ToList());
        }

        [TestMethod]
        public void GivenServiceTypeHasExistingCategoryAssociations_WhenGenerateEditViewModel_ThenViewModelCategoryListHasCorrectSelectedItems()
        {
            var expected = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            ServiceTypeModel viewModel = Target.GenerateEditViewModel(User, 3);

            List<int> actual = viewModel.Categories.SelectedValues.Cast<int>().ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenServiceTypeHasExistingServiceOfferingAssociationsWithPrograms_WhenGenerateEditViewModel_ThenViewModelProgramListHasCorrectSelectedItems()
        {
            var expected = Data.ServiceTypes[0].ServiceOfferings.Where(s => s.IsActive && s.Program.IsActive).Select(so => so.ProgramId).Distinct();
            var serviceType = Data.ServiceTypes[0];

            ServiceTypeModel viewModel = Target.GenerateEditViewModel(User, 1);

            var actual = viewModel.Programs.SelectedValues.Cast<int>().ToList();
            CollectionAssert.AreEqual(expected.ToList(), actual);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenServiceTypeUpdated_AndSaved()
        {
            Data.StudentAssignedOfferings.Clear();
            var expected = Data.ServiceTypes[0];
            var viewModel = new ServiceTypeModel { Id = 1, Name = "Bob" };

            Target.Edit(viewModel);

            Repositories.MockServiceTypeRepository.AssertWasCalled(m => m.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidServiceTypeId_WhenEdit_ThenThrowException()
        {
            ServiceTypeModel viewModel = new ServiceTypeModel { Id = 100 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(viewModel));
        }

        [TestMethod]
        public void GivenServiceTypeCategoryAssociationsWereMade_WhenEdit_ThenServiceTypeHasSelectedServices()
        {
            int[] selectedServices = new int[] { 5, 6 };
            List<Category> expected = Data.Categories.Where(c => selectedServices.Contains(c.Id)).ToList();
            List<Category> actual = new List<Category>();
            ServiceTypeModel viewModel = new ServiceTypeModel { Id = 2, Name = "Mentoring", SelectedCategories = selectedServices };
            Repositories.MockServiceTypeRepository.Expect(m => m.AddLink(null, null)).IgnoreArguments().Do(new Action<ServiceType, Category>((p, s) => actual.Add(s)));

            Target.Edit(viewModel);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeServiceOfferingAssociationsWithProgramsWereMade_WhenEdit_ThenServiceTypeHasSelectedPrograms()
        {
            var selectedPrograms = new int[] { 1, 2 };
            var expected = Data.Programs.Where(s => selectedPrograms.Contains(s.Id)).ToList();
            var viewModel = new ServiceTypeModel { Id = 2, SelectedPrograms = selectedPrograms };

            Target.Edit(viewModel);

            var actual = Repositories.MockServiceTypeRepository.Items.Single(s => s.Id == viewModel.Id).ServiceOfferings.Where(so => so.IsActive).Select(s => s.Program).Distinct();
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenServiceTypeIsTheLastOneOnAProgram_WhenEdit_ThenProgramIsDeactivated()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];
            var viewModel = new ServiceTypeModel { Id = 1, SelectedPrograms = new int[] { 2 } };

            Target.Edit(viewModel);

            Assert.IsFalse(programToDeactivate.IsActive);
        }

        [TestMethod]
        public void GivenServiceTypeIsTheLastOneOnAProgram_WhenEdit_ThenAssociatedSchoolLinksAreDeleted()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];
            var viewModel = new ServiceTypeModel { Id = 1, SelectedPrograms = new int[] { 2 } };

            Target.Edit(viewModel);

            Assert.AreEqual(0, programToDeactivate.Schools.Count());
        }

        [TestMethod]
        public void GivenServiceTypeServiceOfferingsToBeDeactivatedHaveStudentAssignedOfferings_WhenEdit_ThenThrowException()
        {
            var selectedPrograms = new int[] { 3, 4 };
            var viewModel = new ServiceTypeModel { Id = 1, SelectedPrograms = selectedPrograms };

            Target.ExpectException<ValidationException>(() => Target.Edit(viewModel));
        }

        [TestMethod]
        public void GivenInvalidServiceTypeId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(100));
        }

        [TestMethod]
        public void GivenValidServiceTypeId_WhenGenerateDeleteViewModel_ThenViewModelIsReturned()
        {
            ServiceType expected = Repositories.MockServiceTypeRepository.Items.Where(m => m.Id == 1).Single();

            var actual = Target.GenerateDeleteViewModel(1);

            Assert.AreEqual(expected.Id, actual.Id);
        }

        [TestMethod]
        public void GivenInvalidServiceTypeIdWhenDelete_ThenThrowExeption()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(100));
        }

        [TestMethod]
        public void GivenValidServiceTypeId_WhenDelete_ThenServiceTypeInactive_AndSaved()
        {
            Data.StudentAssignedOfferings.Clear();
            var expected = Data.ServiceTypes[2];

            Target.Delete(expected.Id);

            Assert.IsFalse(expected.IsActive);
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenServiceTypeIsTheLastOneOnAProgram_WhenDelete_ThenProgramIsDeactivated()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];

            Target.Delete(1);

            Assert.IsFalse(programToDeactivate.IsActive);
        }

        [TestMethod]
        public void GivenServiceTypeAssociatedWithProgramWithServiceOfferingsThatHaveStudentAssignedOfferings_WhenDelete_ThenThrowException()
        {
            Target.ExpectException<ValidationException>(() => Target.Delete(1));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenAddedServiceTypeMatchesViewModelState()
        {
            ServiceType actualAdded = null;
            ServiceTypeModel expectedState = new ServiceTypeModel
            {
                Id = 3,
                Name = "Joe",
                Description = "Description",
                SelectedCategories = new List<int> { 1, 2 }
            };
            Repositories.MockServiceTypeRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<ServiceType>(p => actualAdded = p));

            Target.Create(expectedState);

            Assert.IsNotNull(actualAdded);
            Assert.AreEqual(expectedState.Name, actualAdded.Name);
            Assert.AreEqual(expectedState.Description, actualAdded.Description);
        }

        [TestMethod]
        public void GivenValidViewModel_AndCategoryAssociationsWereMade_WhenCreate_ThenServiceTypeHasSelectedCategories()
        {
            int[] selectedCategories = new int[] { 1, 3 };
            List<Category> expected = Data.Categories.Where(c => selectedCategories.Contains(c.Id)).ToList();
            ServiceTypeModel viewModel = new ServiceTypeModel { SelectedCategories = selectedCategories };
            List<Category> actual = new List<Category>();
            Repositories.MockServiceTypeRepository.Expect(m => m.AddLink(null, null)).IgnoreArguments().Do(new Action<ServiceType, Category>((p, s) => actual.Add(s)));

            Target.Create(viewModel);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenValidViewModel_AndProgramAssociationsWereMade_WhenCreate_ThenServiceTypeHasSelectedPrograms()
        {
            var selectedPrograms = new int[] { 1, 2 };
            var expected = Data.Programs.Where(s => selectedPrograms.Contains(s.Id)).ToList();
            var viewModel = new ServiceTypeModel { SelectedPrograms = selectedPrograms };
            
            Target.Create(viewModel);

            Repositories.MockServiceTypeRepository.AssertWasCalled(m => m.Add(Arg<ServiceType>.Matches(s => s.ServiceOfferings.Select(so => so.Program).Distinct().SequenceEqual(expected))));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<ServiceType> dataTable = MockRepository.GenerateMock<IClientDataTable<ServiceType>>();
            var expectedQuery = Data.ServiceTypes.Where(s => s.IsActive);
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<ServiceType>>.Matches(s => s.Where(st => st.IsActive).SequenceEqual(expectedQuery)), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenValidUser_WhenSetPrivacy_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("SetServiceTypePrivacy")).Return(permission);

            Target.SetPrivacy(User, 1, false);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenInvalidServiceTypeId_WhenSetPrivacy_ThenThrowException()
        {
            PermissionFactory.Current.Expect(m => m.Create("SetServiceTypePrivacy")).Return(MockRepository.GenerateMock<IPermission>());

            Target.ExpectException<EntityNotFoundException>(() => Target.SetPrivacy(User, 242241, false));
        }

        [TestMethod]
        public void GivenUserHasAccessRight_AndValidServiceTypeId_WhenSetPrivacy_ThenServiceTypePrivacyFlagSet()
        {
            PermissionFactory.Current.Expect(m => m.Create("SetServiceTypePrivacy")).Return(MockRepository.GenerateMock<IPermission>());
            ServiceType expected = Repositories.MockServiceTypeRepository.Items.Where(m => m.Id == 1).Single();
            expected.IsPrivate = false;

            Target.SetPrivacy(User, 1, true);

            Assert.IsTrue(expected.IsPrivate);
        }

        [TestMethod]
        public void GivenUserHasAccessRight_AndValidServiceTypeId_WhenSetPrivacy_ThenServiceTypeIsUpdated_AndSaved()
        {
            int serviceTypeId = 1;
            var expected = Data.ServiceTypes.Single(s => s.Id == serviceTypeId);
            PermissionFactory.Current.Expect(m => m.Create("SetServiceTypePrivacy")).Return(MockRepository.GenerateMock<IPermission>());

            Target.SetPrivacy(User, serviceTypeId, true);

            Repositories.MockServiceTypeRepository.AssertWasCalled(s => s.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(s => s.Save());
        }

        [TestMethod]
        public void GivenNullViewModel_WhenPopulateViewModel_ThenArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.PopulateViewModel(User, null));
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModel_ThenCategoriesAndProgramsListsSet()
        {
            ServiceTypeModel model = new ServiceTypeModel();

            Target.PopulateViewModel(User, model);

            Assert.IsNotNull(model.Categories);
            Assert.IsNotNull(model.Programs);
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModel_ThenCategoriesAndProgramsPopulated()
        {
            var expectedCategories = Repositories.MockCategoryRepository.Items.ToList();
            var expectedPrograms = Repositories.MockProgramRepository.Items.Where(p => p.IsActive).ToList();
            ServiceTypeModel model = new ServiceTypeModel();

            Target.PopulateViewModel(User, model);

            CollectionAssert.AreEquivalent(expectedCategories, model.Categories.Items.Cast<Category>().ToList());
            CollectionAssert.AreEquivalent(expectedPrograms, model.Programs.Items.Cast<Program>().ToList());
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModel_ThenCategoriesAndProgramsListPropertiesCorrect()
        {
            ServiceTypeModel model = new ServiceTypeModel();

            Target.PopulateViewModel(User, model);

            Assert.AreEqual("Id", model.Categories.DataValueField);
            Assert.AreEqual("Id", model.Programs.DataValueField);
            Assert.AreEqual("Name", model.Categories.DataTextField);
            Assert.AreEqual("Name", model.Programs.DataTextField);
        }

        [TestMethod]
        public void GivenViewModel_AndViewModelHasSelectedCategoriesAndPrograms_WhenPopulateViewModel_ThenSelectListsHaveSelectedCategoriesAndPrograms()
        {
            var expectedCategorySelectedValues = new[] { 1, 3 };
            var expectedProgramSelectedValues = new[] { 2, 3 };
            ServiceTypeModel model = new ServiceTypeModel
            {
                SelectedCategories = expectedCategorySelectedValues,
                SelectedPrograms = expectedProgramSelectedValues
            };

            Target.PopulateViewModel(User, model);

            CollectionAssert.AreEqual(expectedCategorySelectedValues, model.Categories.SelectedValues.Cast<int>().ToList());
            CollectionAssert.AreEqual(expectedProgramSelectedValues, model.Programs.SelectedValues.Cast<int>().ToList());
        }

        [TestMethod]
        public void GivenNullUser_WhenGenerateCreateViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GenerateCreateViewModel(null));
        }

        [TestMethod]
        public void GivenNullUser_WhenGenerateEditViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.GenerateEditViewModel(null, 1));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenAnArgumentNullExceptionIsThrown()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => Target.Create(null));
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenReturnInstance()
        {
            ServiceTypeSelectorModel actual = Target.GenerateSelectorViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenViewModelContainsSchools()
        {
            List<ServiceType> expected = Repositories.MockServiceTypeRepository.Items.Where(s => s.IsActive).ToList();

            ServiceTypeSelectorModel actual = Target.GenerateSelectorViewModel();

            CollectionAssert.AreEqual(expected, actual.ServiceTypes.Items.Cast<ServiceType>().ToList());
        }

        [TestMethod]
        public void WhenGenerateSelectorViewModel_ThenSchoolListPropertiesCorrect()
        {
            ServiceTypeSelectorModel actual = Target.GenerateSelectorViewModel();

            Assert.AreEqual("Id", actual.ServiceTypes.DataValueField);
            Assert.AreEqual("Name", actual.ServiceTypes.DataTextField);
        }
    }
}
