using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ProgramManagerTest : BaseManagerTest
    {
        private ProgramManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ProgramManager(Repositories.MockRepositoryContainer, MockDataTableBinder);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ProgramManager(null, MockDataTableBinder));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ProgramManager(Repositories.MockRepositoryContainer, null));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<Program> dataTable = MockRepository.GenerateMock<IClientDataTable<Program>>();
            var expectedQuery = Data.Programs.Where(p => p.IsActive);
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<Program>>.Matches(p => p.Where(pr => pr.IsActive).SequenceEqual(expectedQuery)), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenViewModelHasProviderSelectListPopulated()
        {
            var expected = Data.Providers.Where(p => p.IsActive).OrderBy(p => p.Name).ToList();
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected, viewModel.Providers.Items.Cast<Provider>().ToList());
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenProviderSelectListTextAndValueMembersSet()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            Assert.AreEqual("Name", viewModel.Providers.DataTextField);
            Assert.AreEqual("Id", viewModel.Providers.DataValueField);
        }

        [TestMethod]
        public void GivenModelHasSelectedProviders_WhenPopulateViewModelLists_ThenProviderSelectListSelectionsMade()
        {
            var expected = new List<int> { 1, 4 };
            ProgramModel viewModel = new ProgramModel { SelectedProviders = expected };

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected, viewModel.Providers.SelectedValues.Cast<int>().ToList());
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenViewModelHasServiceTypeSelectListPopulated()
        {
            var expected = Data.ServiceTypes.Where(s => s.IsActive).OrderBy(s => s.Name);
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected.ToList(), viewModel.ServiceTypes.Items.Cast<ServiceType>().ToList());
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenServiceTypeSelectListTextAndValueMembersSet()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            Assert.AreEqual("Name", viewModel.ServiceTypes.DataTextField);
            Assert.AreEqual("Id", viewModel.ServiceTypes.DataValueField);
        }

        [TestMethod]
        public void GivenModelHasSelectedServiceTypes_WhenPopulateViewModelLists_ThenServiceTypeSelectListSelectionsMade()
        {
            var expected = new List<int> { 1, 4 };
            ProgramModel viewModel = new ProgramModel { SelectedServiceTypes = expected };

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected, viewModel.ServiceTypes.SelectedValues.Cast<int>().ToList());
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenViewModelHasSchoolSelectListPopulated()
        {
            var expected = Data.Schools.OrderBy(s => s.Name);
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected.ToList(), viewModel.Schools.Items.Cast<School>().ToList());
        }

        [TestMethod]
        public void WhenPopulateViewModelLists_ThenSchoolSelectListTextAndValueMembersSet()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.PopulateViewModelLists(viewModel);

            Assert.AreEqual("Name", viewModel.Schools.DataTextField);
            Assert.AreEqual("Id", viewModel.Schools.DataValueField);
        }

        [TestMethod]
        public void GivenModelHasSelectedSchools_WhenPopulateViewModelLists_ThenSchoolSelectListSelectionsMade()
        {
            var expected = new List<int> { 1, 4 };
            ProgramModel viewModel = new ProgramModel { SelectedSchools = expected };

            Target.PopulateViewModelLists(viewModel);

            CollectionAssert.AreEqual(expected, viewModel.Schools.SelectedValues.Cast<int>().ToList());
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelReturned()
        {
            ProgramModel actual = Target.GenerateCreateViewModel();

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenViewModelNull_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null));
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenProgramAddedToRepository()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Is.NotNull));
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenChangesSaved()
        {
            ProgramModel viewModel = new ProgramModel();

            Target.Create(viewModel);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenViewModel_WhenCreate_ThenStateCopiedToEntityToCreate()
        {
            ProgramModel viewModel = new ProgramModel
            {
                Name = "Mortal Kombat",
                StartDate = new DateTime(2001, 4, 6),
                EndDate = new DateTime(2001, 10, 11),
                Purpose = "Don't worry, it is just a video game.  It teaches important life lessons....really.",
                ContactEmail = "bob@bob.bob",
                ContactName = "Bob",
                ContactPhone = "123-456-7890"
            };

            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Matches(p => AssertPropertiesMatch(viewModel, p))));
        }

        [TestMethod]
        public void GivenViewModelWithSelectedSchools_WhenCreate_ThenSchoolsListUpdated()
        {
            var expected = new int[] { 1, 2 };
            ProgramModel viewModel = new ProgramModel { SelectedSchools = expected, SelectedProviders = new int[] { 1 }, SelectedServiceTypes = new int[] { 1 } };

            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Matches(p => p.Schools.Select(s => s.Id).SequenceEqual(expected))));
        }

        [TestMethod]
        public void GivenViewModelProviderAssociationsWereMade_WhenCreate_ThenProgramHasServiceOfferingsWithSelectedProviders()
        {
            var expected = new int[] { 1, 2 };
            ProgramModel viewModel = new ProgramModel { SelectedProviders = expected, SelectedServiceTypes = new int[] { 1 } };
            
            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Matches(p => p.ServiceOfferings.Select(s => s.ProviderId).Distinct().SequenceEqual(expected))));
        }

        [TestMethod]
        public void GivenViewModelServiceTypeAssociationsWereMade_WhenCreate_ThenProgramHasServiceOfferingsWithSelectedServiceTypes()
        {
            var expected = new int[] { 1, 4 };
            ProgramModel viewModel = new ProgramModel { SelectedServiceTypes = expected, SelectedProviders = new [] { 1 } };

            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Matches(p => p.ServiceOfferings.Select(s => s.ServiceTypeId).Distinct().SequenceEqual(expected))));
        }

        [TestMethod]
        public void GivenMultipleServiceTypesAndPrograms_WhenCreate_ThenCorrespondingServiceOfferingsAreMade()
        {
            ProgramModel viewModel = new ProgramModel { SelectedProviders = new int[] { 3 }, SelectedServiceTypes = new int[] { 1, 2, 3 } };
            List<ServiceOffering> expectedOfferings = new List<ServiceOffering> {
                new ServiceOffering { ProviderId = 3, ServiceTypeId = 1 },
                new ServiceOffering { ProviderId = 3, ServiceTypeId = 2 },
                new ServiceOffering { ProviderId = 3, ServiceTypeId = 3 }
            };

            Target.Create(viewModel);

            foreach(var expected in expectedOfferings)
            {
                Repositories.MockServiceOfferingRepository.AssertWasCalled(m => m.Add(Arg<ServiceOffering>.Matches(s => s.ServiceTypeId.Equals(expected.ServiceTypeId) && s.ProviderId.Equals(expected.ProviderId))));
            }
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenIsActiveSetToTrue()
        {
            ProgramModel viewModel = new ProgramModel { SelectedProviders = new int[] { 3 }, SelectedServiceTypes = new int[] { 1 } };

            Target.Create(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Add(Arg<Program>.Matches(p => p.IsActive == true)));
        }

        [TestMethod]
        public void GivenProgramNameAlreadyExistsButIsInactive_WhenCreate_ThenProgramIsReactivated()
        {
            var inactive = Data.Programs.Where(p => !p.IsActive).First();
            var expectedState = new ProgramModel { Name = inactive.Name, SelectedProviders = new int[] { 1 }, SelectedServiceTypes = new int[] { 1 } };

            Target.Create(expectedState);

            Assert.IsTrue(inactive.IsActive);
        }

        [TestMethod]
        public void GivenProgramNameAlreadyExistsButIsInactive_AndItWillCreateAServiceOfferingThatIsInactive_WhenCreate_ThenServiceOfferingReactivated()
        {
            var inactive = Data.Programs.Single(p => p.Id == 5);
            var inactiveServiceOffering = Data.ServiceOfferings.Single(p => p.Id == 14);
            var expectedState = new ProgramModel { Name = inactive.Name, SelectedProviders = new int[] { inactiveServiceOffering.ProviderId }, SelectedServiceTypes = new int[] { inactiveServiceOffering.ServiceTypeId } };
            
            Target.Create(expectedState);

            Assert.IsTrue(inactiveServiceOffering.IsActive);

        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelReturned()
        {
            ProgramModel actual = Target.GenerateEditViewModel(1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenInvalidProgramId_WhenGenerateEditViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(10001));
        }

        [TestMethod]
        public void GivenInactiveProgramId_WhenGenerateEditViewModel_ThenThrowException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(5));
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelStateMatchesEntity()
        {
            int id = 2;
            Program expectedState = Data.Programs.Single(p => p.Id == id);

            ProgramModel actual = Target.GenerateEditViewModel(id);

            Assert.AreEqual(expectedState.Name, actual.Name);
            Assert.AreEqual(expectedState.StartDate, actual.StartDate);
            Assert.AreEqual(expectedState.EndDate, actual.EndDate);
            Assert.AreEqual(expectedState.Purpose, actual.Purpose);
            Assert.AreEqual(expectedState.ContactInfo.Email, actual.ContactEmail);
            Assert.AreEqual(expectedState.ContactInfo.Name, actual.ContactName);
            Assert.AreEqual(expectedState.ContactInfo.Phone, actual.ContactPhone);
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelHasSelectedProviders()
        {
            int id = 4;
            Program expectedState = Data.Programs.Single(p => p.Id == id);

            ProgramModel actual = Target.GenerateEditViewModel(id);

            CollectionAssert.AreEqual(expectedState.ServiceOfferings.Where(s => s.IsActive).Select(s => s.Provider).Select(p => p.Id).ToList(), actual.SelectedProviders.ToList());
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelHasSelectedServiceTypes()
        {
            int id = 4;
            Program expectedState = Data.Programs.Single(p => p.Id == id);

            ProgramModel actual = Target.GenerateEditViewModel(id);

            CollectionAssert.AreEqual(expectedState.ServiceOfferings.Where(s => s.IsActive).Select(s => s.ServiceType).Select(t => t.Id).ToList(), actual.SelectedServiceTypes.ToList());
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateEditViewModel_ThenViewModelHasSelectedSchools()
        {
            int id = 1;
            Program expectedState = Data.Programs.Single(p => p.Id == id);

            ProgramModel actual = Target.GenerateEditViewModel(id);

            CollectionAssert.AreEqual(expectedState.Schools.Select(t => t.Id).ToList(), actual.SelectedSchools.ToList());
        }

        [TestMethod]
        public void GivenViewModelNull_WhenEdit_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Edit(null));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenProgramUpdatedInRepository()
        {
            int id = 2;
            Program expected = Data.Programs.Single(p => p.Id == id);
            ProgramModel viewModel = new ProgramModel { Id = id };

            Target.Edit(viewModel);

            Repositories.MockProgramRepository.AssertWasCalled(m => m.Update(expected));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenChangesSaved()
        {
            ProgramModel viewModel = new ProgramModel { Id = 2 };

            Target.Edit(viewModel);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenViewModel_WhenEdit_ThenStateCopiedToEntityToUpdate()
        {
            int id = 2;
            ProgramModel viewModel = new ProgramModel
            {
                Id = id,
                Name = "Mortal Combat",
                StartDate = new DateTime(2001, 4, 6),
                EndDate = new DateTime(2001, 10, 11),
                Purpose = "Don't worry, it is just a video game.  It teaches important life lessons....really.",
                ContactEmail = "bob@bob.bob",
                ContactName = "Bob",
                ContactPhone = "123-456-7890"
            };
            Program toUpdate = Data.Programs.Single(p => p.Id == id);

            Target.Edit(viewModel);

            Assert.AreEqual(viewModel.Name, toUpdate.Name);
            Assert.AreEqual(viewModel.StartDate, toUpdate.StartDate);
            Assert.AreEqual(viewModel.EndDate, toUpdate.EndDate);
            Assert.AreEqual(viewModel.Purpose, toUpdate.Purpose);
            Assert.AreEqual(viewModel.ContactEmail, toUpdate.ContactInfo.Email);
            Assert.AreEqual(viewModel.ContactName, toUpdate.ContactInfo.Name);
            Assert.AreEqual(viewModel.ContactPhone, toUpdate.ContactInfo.Phone);
        }

        [TestMethod]
        public void GivenNotFoundViewModel_WhenEdit_ThenThrowEntityNotFound()
        {
            ProgramModel viewModel = new ProgramModel { Id = 23828 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(viewModel));
        }

        [TestMethod]
        public void GivenViewModelWithEditedSchools_WhenEdit_ThenSchoolsListUpdated()
        {
            var expected = new int[] { 1, 2 };
            ProgramModel viewModel = new ProgramModel { Id = 1, SelectedSchools = expected, SelectedProviders = new int[] { 1 }, SelectedServiceTypes = new int[] { 1 } };
            Program toUpdate = Data.Programs.Single(p => p.Id == 1);

            Target.Edit(viewModel);

            CollectionAssert.AreEqual(expected, toUpdate.Schools.Select(s => s.Id).ToArray());
        }

        [TestMethod]
        public void GivenViewModelProviderAssociationsWereMade_WhenEdit_ThenProgramHasServiceOfferingsWithSelectedProviders()
        {
            int id = 3;
            Program toUpdate = Data.Programs.Single(p => p.Id == id);
            var expected = new int[] { 1, 2 };
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedProviders = expected, SelectedServiceTypes = new [] { 1 } };

            Target.Edit(viewModel);

            var actual = toUpdate.ServiceOfferings.Select(s => s.ProviderId).Distinct();
            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenViewModelServiceTypeAssociationsWereMade_WhenEdit_ThenProgramHasServiceOfferingsWithSelectedServiceTypes()
        {
            int id = 2;
            Program toUpdate = Data.Programs.Single(p => p.Id == id);
            var expected = new int[] { 1, 4 };
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedServiceTypes = expected, SelectedProviders = new [] { 1 } };

            Target.Edit(viewModel);

            var actual = toUpdate.ServiceOfferings.Select(s => s.ServiceTypeId).Distinct();
            CollectionAssert.AreEquivalent(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenProviderRemovedFromProgram_WhenEdit_ThenServiceOfferingsAssociatedToProviderSetInactive()
        {
            int id = 2;
            ServiceOffering expectedInactive = Data.ServiceOfferings.Single(s => s.Id == 2);
            ServiceOffering expectedActive = Data.ServiceOfferings.Single(s => s.Id == 3);
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedServiceTypes = new int[] { 4 }, SelectedProviders = new int[] { 1 } };

            Target.Edit(viewModel);

            Assert.IsTrue(expectedActive.IsActive);
            Assert.IsFalse(expectedInactive.IsActive);
        }

        [TestMethod]
        public void GivenProviderRemovedFromProgram_AndServiceOfferingsToDeactivateHaveStudentAssignedOfferings_WhenEdit_ThenThrowException()
        {
            int id = 2;
            ServiceOffering expectedInactive = Data.ServiceOfferings.Single(s => s.Id == 2);
            expectedInactive.StudentAssignedOfferings.Add(new StudentAssignedOffering { IsActive = true } );
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedServiceTypes = new int[] { 4 }, SelectedProviders = new int[] { 1 } };

            Target.ExpectException<ValidationException>(() => Target.Edit(viewModel));
        }

        [TestMethod]
        public void GivenServiceTypeAddedToProgram_WhenEdit_ThenNewServiceOfferingsCreated()
        {
            int id = 2;
            Program toUpdate = Data.Programs.Single(p => p.Id == id);
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedServiceTypes = new int[] { 1, 4 }, SelectedProviders = new int[] { 1 } };

            Target.Edit(viewModel);

            Repositories.MockServiceOfferingRepository.AssertWasCalled(m => m.Add(Arg<ServiceOffering>.Matches(s => s.Program == toUpdate && s.ProviderId == 1 && s.ServiceTypeId == 1)));
        }

        [TestMethod]
        public void GivenServiceTypeProviderProgramAlreadyExists_AndOfferingIsInactive_AndServiceTypeAddedToProgram_WhenEdit_ThenServiceOfferingIsActive()
        {
            int id = 2;
            ServiceOffering newOffering = new ServiceOffering { ProviderId = 1, ServiceTypeId = 1, ProgramId = id, IsActive = false };
            Data.ServiceOfferings.Add(newOffering);
            ProgramModel viewModel = new ProgramModel { Id = id, SelectedServiceTypes = new int[] { 1, 4 }, SelectedProviders = new int[] { 1 } };

            Target.Edit(viewModel);

            Assert.IsTrue(newOffering.IsActive);
        }

        [TestMethod]
        public void GivenViewModelHasInactiveProgramId_WhenEdit_ThenThrowException()
        {
            ProgramModel viewModel = new ProgramModel { Id = 5, SelectedServiceTypes = new int[] { 1, 4 }, SelectedProviders = new int[] { 1 } };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(viewModel));
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateDeleteViewModel_ThenViewModelReturned()
        {
            ProgramModel actual = Target.GenerateDeleteViewModel(1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenInvalidProgramId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(10001));
        }

        [TestMethod]
        public void GivenValidProgramId_WhenGenerateDeleteViewModel_ThenViewModelHasProgramName()
        {
            int id = 4;
            Program expectedState = Data.Programs.Single(p => p.Id == id);

            ProgramModel actual = Target.GenerateDeleteViewModel(id);

            Assert.AreEqual(expectedState.Name, actual.Name);
        }

        [TestMethod]
        public void GivenValidProgramId_WhenDelete_ThenProgramSetInactive()
        {
            int id = 4;
            Program toDelete = Data.Programs.Single(p => p.Id == id);

            Target.Delete(id);

            Assert.IsFalse(toDelete.IsActive);
        }

        [TestMethod]
        public void GivenValidProgramId_WhenDelete_ThenChangesSaved()
        {
            Target.Delete(4);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidProgramId_WhenDelete_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(489));
        }

        [TestMethod]
        public void GivenServiceOfferingsAssociatedWithProgram_WhenDelete_ThenServiceOfferingsAreDeleted()
        {
            Data.StudentAssignedOfferings.Clear();
            var expectedOfferings = Data.ServiceOfferings.Where(s => s.ProgramId == 1);

            Target.Delete(1);

            foreach (var expected in expectedOfferings)
            {
                expected.IsActive = false;
            }
        }

        [TestMethod]
        public void GivenProgramAssociatedWithServiceOfferingsWithStudentAssignedOfferings_WhenDelete_ThenThrowException() 
        {
            Target.ExpectException<ValidationException>(() => Target.Delete(1));
        }

        [TestMethod]
        public void GivenProgramWithSchools_WhenDelete_ThenProgramSchoolAssociationsRemoved()
        {
            Data.StudentAssignedOfferings.Clear();
            var expected = Data.Programs.First();

            Target.Delete(expected.Id);

            Assert.AreEqual(0, expected.Schools.Count());
        }

        [TestMethod]
        public void GivenMatchingPrograms_WhenSearchProgramNames_ThenListOfProgramNamesIsReturned()
        {
            List<string> expected = new List<string> { "Test Program 3 - After School Swimming", "Test Program 4 - After School Wrestling" };

            var actual = Target.SearchProgramNames("After School");

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenNoMatchingPrograms_WhenSearchProgramNames_ThenEmptyListIsReturned()
        {
            List<string> expected = new List<string>();

            var actual = Target.SearchProgramNames("non-matching term");

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenInactivePrograms_WhenSearchProgramNames_ThenEmptyListIsReturned()
        {
            List<string> expected = new List<string>();

            var actual = Target.SearchProgramNames(Data.Programs.Where(s => !s.IsActive).First().Name);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_WhenValidate_ThenThrowValidationException()
        {
            var model = new ProgramModel { Name = Data.Programs[0].Name };

            Target.ExpectException<ValidationException>(() => Target.Validate(model));
        }

        [TestMethod]
        public void GivenViewModelHasUniqueName_WhenValidate_ThenSucceed()
        {
            var model = new ProgramModel { Name = "this doesn't already exist!" };

            Target.Validate(model);
        }

        private static bool AssertPropertiesMatch(ProgramModel expectedState, Program actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.Name, actualState.Name);
            Assert.AreEqual(expectedState.StartDate, actualState.StartDate);
            Assert.AreEqual(expectedState.EndDate, actualState.EndDate);
            Assert.AreEqual(expectedState.Purpose, actualState.Purpose);
            Assert.AreEqual(expectedState.ContactEmail, actualState.ContactInfo.Email);
            Assert.AreEqual(expectedState.ContactName, actualState.ContactInfo.Name);
            Assert.AreEqual(expectedState.ContactPhone, actualState.ContactInfo.Phone);
            return true;
        }
    }
}
