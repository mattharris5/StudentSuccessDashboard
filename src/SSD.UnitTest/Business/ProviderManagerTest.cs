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
    public class ProviderManagerTest : BaseManagerTest
    {
        private ProviderManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ProviderManager(Repositories.MockRepositoryContainer, MockDataTableBinder);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ProviderManager(null, MockDataTableBinder));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenIConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ProviderManager(Repositories.MockRepositoryContainer, null));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<Provider> dataTable = MockRepository.GenerateMock<IClientDataTable<Provider>>();
            var expectedQuery = Data.Providers.Where(p => p.IsActive);
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<Provider>>.Matches(p => p.Where(pr => pr.IsActive).SequenceEqual(expectedQuery)), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenPopulateViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.PopulateViewModel(null));
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModel_ThenViewModelContainsActiveProgramList()
        {
            var model = new ProviderModel();

            Target.PopulateViewModel(model);

            CollectionAssert.AreEqual(Data.Programs.Where(p => p.IsActive).ToList(), model.Programs.Items.Cast<Program>().ToList());
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_WhenValidate_ThenThrowValidationException()
        {
            var model = new ProviderModel { Name = "YMCA" };

            Target.ExpectException<ValidationException>(() => Target.Validate(model));
        }

        [TestMethod]
        public void GivenViewModelHasUniqueName_WhenValidate_ThenSucceed()
        {
            var model = new ProviderModel { Name = "this doesn't already exist!" };

            Target.Validate(model);
        }

        [TestMethod]
        public void GivenViewModelHasDuplicateName_AndDuplicateIsInactive_WhenValidate_ThenSucceed()
        {
            var model = new ProviderModel { Name = "The Math Hut" };

            Target.Validate(model);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(User, null));
        }

        [TestMethod]
        public void GivenAViewModel_WhenCreate_ThenChangesSaved()
        {
            var viewModel = new ProviderModel();

            Target.Create(User, viewModel);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenAViewModel_WhenCreate_ThenAddedProviderStateMatchesViewModelState()
        {
            var expectedState = new ProviderModel { Name = "Bob's Tutoring", Website = "www.bob.com", Address = new Address { City = "Bob City", State = "BB", Street = "123 Bob Street", Zip = "12345" }, Contact = new Contact { Email = "bob@bob.com", Name = "Bob", Phone = "614-444-4444" } };
            
            Target.Create(User, expectedState);

            Repositories.MockProviderRepository.AssertWasCalled(m => m.Add(Arg<Provider>.Matches(p => AssertPropertieMatch(expectedState, p))));
        }

        [TestMethod]
        public void GivenProviderNameAlreadyExistsButIsInactive_WhenCreate_ThenProviderIsReactivated()
        {
            var inactive = Data.Providers.Single(p => p.Id == 4 && !p.IsActive);
            var expectedState = new ProviderModel { Name = inactive.Name, Website = "www.bob.com", Address = new Address { City = "Bob City", State = "BB", Street = "123 Bob Street", Zip = "12345" }, Contact = new Contact { Email = "bob@bob.com", Name = "Bob", Phone = "614-444-4444" } };

            Target.Create(User, expectedState);

            Assert.IsTrue(inactive.IsActive);
        }

        [TestMethod]
        public void GivenProviderNameAlreadyExistsButIsInactive_WhenCreate_ThenProviderNotAdded()
        {
            var inactive = Data.Providers.Single(p => p.Id == 4 && !p.IsActive);
            var expectedState = new ProviderModel { Name = inactive.Name, Website = "www.bob.com", Address = new Address { City = "Bob City", State = "BB", Street = "123 Bob Street", Zip = "12345" }, Contact = new Contact { Email = "bob@bob.com", Name = "Bob", Phone = "614-444-4444" } };

            Target.Create(User, expectedState);

            Repositories.MockProviderRepository.AssertWasNotCalled(m => m.Add(null), options => options.IgnoreArguments());
        }

        [TestMethod]
        public void GivenProviderNameAlreadyExistsButIsInactive_WhenCreate_ThenProviderIdRetained()
        {
            int expectedId = 4;
            var inactive = Data.Providers.Single(p => p.Id == expectedId && !p.IsActive);
            var expectedState = new ProviderModel { Name = inactive.Name, Website = "www.bob.com", Address = new Address { City = "Bob City", State = "BB", Street = "123 Bob Street", Zip = "12345" }, Contact = new Contact { Email = "bob@bob.com", Name = "Bob", Phone = "614-444-4444" } };

            Target.Create(User, expectedState);

            Assert.AreEqual(expectedId, inactive.Id);
        }

        [TestMethod]
        public void GivenProgramSelection_WhenCreate_ThenServiceOfferingsCreatedForNewProgramMappings()
        {
            Provider added = null;
            var programs = Data.Programs.Take(2);
            var viewModel = new ProviderModel { SelectedPrograms = programs.Select(p => p.Id).ToList() };
            Repositories.MockProviderRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<Provider>(p => { added = p; }));

            Target.Create(User, viewModel);

            foreach (Program program in programs)
            {
                foreach (ServiceType serviceType in program.ServiceOfferings.Select(s => s.ServiceType))
                {
                    Repositories.MockServiceOfferingRepository.AssertWasCalled(m => m.Add(Arg<ServiceOffering>.Matches(s => s.Provider == added && s.ProgramId == program.Id && s.ServiceTypeId == serviceType.Id && s.IsActive)));
                }
            }
        }

        [TestMethod]
        public void GivenAViewModel_AndProgramsAreSelected_WhenCreate_ThenProviderHasSelectedPrograms()
        {
            var selectedPrograms = new int[] { 1, 2 };
            var expected = Data.Programs.Where(s => selectedPrograms.Contains(s.Id)).ToList();
            var viewModel = new ProviderModel { SelectedPrograms = selectedPrograms };

            Target.Create(User, viewModel);

            Repositories.MockProviderRepository.AssertWasCalled(m => m.Add(Arg<Provider>.Matches(p => p.ServiceOfferings.Select(s => s.Program).Distinct().SequenceEqual(expected))));
        }

        [TestMethod]
        public void GivenValidProviderId_WhenGenerateEditViewModel_ThenAViewModelIsCreated()
        {
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", 1)).Return(MockRepository.GenerateMock<IPermission>());

            ProviderModel result = Target.GenerateEditViewModel(User, 1);

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void GivenValidProviderId_WhenGenerateEditViewModel_ThenAttemptAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", 1)).Return(permission);

            Target.GenerateEditViewModel(User, 1);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidProviderId_WhenGenerateEditViewModel_ThenViewModelSelectListsPopulated()
        {
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", 1)).Return(MockRepository.GenerateMock<IPermission>());

            ProviderModel actual = Target.GenerateEditViewModel(User, 1);

            CollectionAssert.AreEqual(Data.Programs.Where(p => p.IsActive).ToList(), actual.Programs.Items.Cast<Program>().ToList());
        }

        [TestMethod]
        public void GivenValidProviderId_AndProviderHasExistingProgramAssociations_WhenGenerateEditViewModel_ThenViewModelProgramListHasCorrectSelectedItems()
        {
            var expected = new List<int> { 1 };
            var provider = Data.Providers[0];
            provider.ServiceOfferings = Data.Programs.Where(s => expected.Contains(s.Id)).Select(s => new ServiceOffering { Program = s }).ToList();
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", provider.Id)).Return(MockRepository.GenerateMock<IPermission>());

            ProviderModel result = Target.GenerateEditViewModel(User, provider.Id);

            var actual = result.Programs.SelectedValues.Cast<int>().ToList();
            CollectionAssert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenInvalidProviderId_WhenGenerateEditViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 100));
        }

        [TestMethod]
        public void GivenInactiveProviderId_WhenGenerateEditViewModel_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 4));
        }

        [TestMethod]
        public void GivenValidProviderViewModel_WhenEdit_ThenProviderUpdatedInRepository_AndSaved()
        {
            Data.StudentAssignedOfferings.Clear();
            var expected = Data.Providers[0];
            var viewModel = new ProviderModel { Id = expected.Id, Name = "YMCA" };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", expected.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Repositories.MockProviderRepository.AssertWasCalled(m => m.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidProviderViewModel_WhenEdit_ThenAttemptGrantAccess()
        {
            Data.StudentAssignedOfferings.Clear();
            ProviderModel viewModel = new ProviderModel { Id = 1, Name = "YMCA" };
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", 1)).Return(permission);

            Target.Edit(User, viewModel);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenAViewModel_AndIdIsNotFound_WhenEdit_ThenThrowEntityNotFoundException()
        {
            var viewModel = new ProviderModel { Id = 100 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(User, viewModel));
        }

        [TestMethod]
        public void GivenAViewModel_AndIdIsInactive_WhenEdit_ThenThrowEntityNotFoundException()
        {
            var viewModel = new ProviderModel { Id = 4 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(User, viewModel));
        }

        [TestMethod]
        public void GivenAnEditedProviderIsSubmittedWithoutAUniqueName_WhenEdit_ThenThrowException()
        {
            var viewModel = new ProviderModel { Id = 1, Name = "Jimbo's Math Shop" };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.ExpectException<ValidationException>(() => Target.Edit(User, viewModel));
        }

        [TestMethod]
        public void GivenProviderProgramAssociationsWereMade_WhenEdit_ThenProviderHasServiceOfferingsWithSelectedProgram()
        {
            var selectedPrograms = new int[] { 1, 2 };
            var expected = Data.Programs.Where(s => selectedPrograms.Contains(s.Id)).ToList();
            var viewModel = new ProviderModel { Id = 2, SelectedPrograms = selectedPrograms };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            var actual = Repositories.MockProviderRepository.Items.Single(p => p.Id == viewModel.Id).ServiceOfferings.Where(o => o.IsActive).Select(s => s.Program).Distinct();
            CollectionAssert.AreEquivalent(expected, actual.ToList());
        }

        [TestMethod]
        public void GivenProgramRemovedFromProvider_WhenEdit_ThenAssociatedServiceOfferingsDeactivated()
        {
            Data.ServiceTypes.Clear();
            Data.ServiceTypes.AddRange(new List<ServiceType> { new ServiceType { Name = "A" }, new ServiceType { Name = "B" } });
            Data.Programs.Clear();
            Data.Programs.AddRange(new List<Program> { new Program { Name = "A", Id = 1 }, new Program { Name = "B", Id = 2 } });
            Provider toEdit = new Provider
            {
                Id = 1,
                IsActive = true,
                ServiceOfferings = new List<ServiceOffering>
                {
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[0],
                        Program = Data.Programs[0],
                        ProgramId = Data.Programs[0].Id,
                        IsActive = true,
                        ProviderId = 1
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[1],
                        Program = Data.Programs[0],
                        ProgramId = Data.Programs[0].Id,
                        IsActive = true,
                        ProviderId = 1
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[0],
                        Program = Data.Programs[1],
                        ProgramId = Data.Programs[1].Id,
                        IsActive = true,
                        ProviderId = 1
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[1],
                        Program = Data.Programs[1],
                        ProgramId = Data.Programs[1].Id,
                        IsActive = true,
                        ProviderId = 1
                    }
                }
            };
            Data.Providers.Clear();
            Data.ServiceOfferings.Clear();
            Data.Providers.Add(toEdit);
            Data.ServiceOfferings.AddRange(toEdit.ServiceOfferings);
            var viewModel = new ProviderModel { SelectedPrograms = new[] { 1 }, Id = 1 };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Assert.IsTrue(Data.ServiceOfferings.Where(s => s.IsActive).All(s => s.ProgramId == 1));
        }

        [TestMethod]
        public void GivenProviderServiceOfferingsToBeDeactivatedHaveStudentAssignedOfferings_WhenEdit_ThenThrowException()
        {
            var selectedPrograms = new int[] { 3, 4 };
            var viewModel = new ProviderModel { Id = 1, SelectedPrograms = selectedPrograms };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.ExpectException<ValidationException>(() => Target.Edit(User, viewModel));
        }

        [TestMethod]
        public void GivenProviderIsTheLastOneOnAProgram_WhenEdit_ThenProgramIsDeactivated()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];
            var viewModel = new ProviderModel { Id = 1, SelectedPrograms = new int[] { 2 } };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Assert.IsFalse(programToDeactivate.IsActive);
        }

        [TestMethod]
        public void GivenProviderIsTheLastOneOnAProgram_WhenEdit_ThenAssociatedSchoolLinksAreDeleted()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];
            var viewModel = new ProviderModel { Id = 1, SelectedPrograms = new int[] { 2 } };
            PermissionFactory.Current.Expect(m => m.Create("EditProvider", viewModel.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Assert.AreEqual(0, programToDeactivate.Schools.Count());
        }

        [TestMethod]
        public void GivenInvalidProviderId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(382));
        }

        [TestMethod]
        public void WhenDeleteProviderIsClicked_ThenAPartialViewResultIsCreated()
        {
            var actual = Target.GenerateDeleteViewModel(1) as ProviderModel;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidProviderId_WhenGenerateDeleteViewModel_ThenProviderMatchingIdIsReturned()
        {
            var actual = Target.GenerateDeleteViewModel(1) as ProviderModel;

            Assert.AreEqual(Data.Providers[0].Id, actual.Id);
        }

        [TestMethod]
        public void GivenPostedProviderDoesNotExist_WhenDelete_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(100));
        }

        [TestMethod]
        public void WhenDelete_ThenItIsSetInactiveAndSaved()
        {
            Data.StudentAssignedOfferings.Clear();
            var expected = Data.Providers[0];

            Target.Delete(expected.Id);

            Assert.IsFalse(expected.IsActive);
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenProviderWithServiceOfferingsWithPrograms_WhenDelete_ThenAssociatedServiceOfferingsDeactivated()
        {
            Data.ServiceTypes.Clear();
            Data.StudentAssignedOfferings.Clear();
            Data.ServiceTypes.AddRange(new List<ServiceType> { new ServiceType { Name = "A" }, new ServiceType { Name = "B" } });
            Data.Programs.Clear();
            Data.Programs.AddRange(new List<Program> { new Program { Name = "A" }, new Program { Name = "B" } });
            Provider toDelete = new Provider
            {
                Id = 1,
                IsActive = true,
                ServiceOfferings = new List<ServiceOffering> 
                {
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[0],
                        ServiceTypeId = Data.ServiceTypes[0].Id,
                        Program = Data.Programs[0],
                        ProgramId = Data.Programs[0].Id,
                        ProviderId = 1,
                        IsActive = true
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[1],
                        ServiceTypeId = Data.ServiceTypes[1].Id,
                        Program = Data.Programs[0],
                        ProgramId = Data.Programs[0].Id,
                        ProviderId = 1,
                        IsActive = true
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[0],
                        ServiceTypeId = Data.ServiceTypes[0].Id,
                        Program = Data.Programs[1],
                        ProgramId = Data.Programs[1].Id,
                        ProviderId = 1,
                        IsActive = true
                    },
                    new ServiceOffering
                    {
                        ServiceType = Data.ServiceTypes[1],
                        ServiceTypeId = Data.ServiceTypes[1].Id,
                        Program = Data.Programs[1],
                        ProgramId = Data.Programs[1].Id,
                        ProviderId = 1,
                        IsActive = true
                    }
                }
            };
            Data.Providers.Clear();
            Data.ServiceOfferings.Clear();
            Data.Providers.Add(toDelete);
            Data.ServiceOfferings.AddRange(toDelete.ServiceOfferings);

            Target.Delete(toDelete.Id);

            Assert.IsTrue(Data.ServiceOfferings.All(s => !s.IsActive));
        }

        [TestMethod]
        public void GivenProviderIsTheLastOneOnAProgram_WhenDelete_ThenProgramIsDeactivated()
        {
            Data.StudentAssignedOfferings.Clear();
            var programToDeactivate = Data.Programs[0];

            Target.Delete(1);

            Assert.IsFalse(programToDeactivate.IsActive);
        }

        [TestMethod]
        public void GivenProviderHasApprovingStudents_WhenDelete_ThenStudentApprovalsRemoved()
        {
            var provider = Data.Providers.Single(p => p.Id == 1);
            var toBeDisassociated = provider.ApprovingStudents.ToArray();
            Assert.IsTrue(toBeDisassociated.Any() && toBeDisassociated.All(s => s.ApprovedProviders.Contains(provider)));
            Data.StudentAssignedOfferings.Clear();

            Target.Delete(1);

            Assert.IsFalse(provider.ApprovingStudents.Any() || toBeDisassociated.All(s => s.ApprovedProviders.Contains(provider)));
        }

        [TestMethod]
        public void GivenProviderHasUserAssociations_WhenDelete_ThenUserAssociationsRemoved()
        {
            var provider = Data.Providers.Single(p => p.Id == 1);
            var toBeDisassociated = provider.UserRoles.ToArray();
            Assert.IsTrue(toBeDisassociated.Any() && toBeDisassociated.All(u => u.Providers.Contains(provider)));
            Data.StudentAssignedOfferings.Clear();

            Target.Delete(1);

            Assert.IsFalse(provider.UserRoles.Any() || toBeDisassociated.All(s => s.Providers.Contains(provider)));
        }

        [TestMethod]
        public void GivenProviderAssociatedWithProgramWithServiceOfferingsThatHaveStudentAssignedOfferings_WhenDelete_ThenThrowException()
        {
            Target.ExpectException<ValidationException>(() => Target.Delete(1));
        }

        [TestMethod]
        public void GivenMatchingNamePart_WhenSearchProviderNames_ThenListOfProviderNamesIsReturned()
        {
            AssertSearchProviderNames("Y", new List<string> { "YMCA" });
        }

        [TestMethod]
        public void GivenInvalidName_WhenSearchProviderNames_ThenAnEmptyListIsReturned()
        {
            AssertSearchProviderNames("L", new List<string>());
        }

        [TestMethod]
        public void GivenMatchingNamePart_AndMatchingProviderIsInactive_WhenSearchProviderNames_ThenAnEmptyListIsReturned()
        {
            AssertSearchProviderNames("Hut", new List<string>());
        }

        private void AssertSearchProviderNames(string searchTerm, List<string> expected)
        {
            var actual = Target.SearchProviderNames(searchTerm);

            CollectionAssert.AreEqual(expected, actual.ToList());
        }

        private static bool AssertPropertieMatch(ProviderModel expectedState, Provider actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.Name, actualState.Name);
            Assert.AreEqual(expectedState.Website, actualState.Website);
            Assert.AreEqual(expectedState.Address, actualState.Address);
            Assert.AreEqual(expectedState.Contact, actualState.Contact);
            return true;
        }
    }
}
