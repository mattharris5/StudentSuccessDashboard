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
    public class ServiceOfferingManagerTest : BaseManagerTest
    {
        private ServiceOfferingManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceOfferingManager(Repositories.MockRepositoryContainer, MockDataTableBinder);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingManager(null, MockDataTableBinder));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceOfferingManager(Repositories.MockRepositoryContainer, null));
        }

        [TestMethod]
        public void GivenUserHasFavorites_WhenGenerateListOptionsViewModel_ThenViewModelHasAListOfFavoriteServiceOfferings()
        {
            Data.ServiceOfferings[1].UsersLinkingAsFavorite.Add(User.Identity.User);
            Data.ServiceOfferings[2].UsersLinkingAsFavorite.Add(User.Identity.User);

            ServiceOfferingListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEquivalent(new ServiceOffering[] { Data.ServiceOfferings[1], Data.ServiceOfferings[2] }, actual.Favorites.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenViewModelHasAListOfServiceTypes()
        {
            ServiceOfferingListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEqual(Data.ServiceTypes.Where(s => s.IsActive).Select(c => c.Name).ToList(), actual.TypeFilterList.ToList());
        }

        [TestMethod]
        public void WhenGenerateListOptionsViewModel_ThenViewModelHasAListOfServiceCategories()
        {
            ServiceOfferingListOptionsModel actual = Target.GenerateListOptionsViewModel(User);

            CollectionAssert.AreEqual(Data.Categories.Select(c => c.Name).ToList(), actual.CategoryFilterList.ToList());
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<ServiceOffering> dataTable = MockRepository.GenerateMock<IClientDataTable<ServiceOffering>>();
            var expectedQuery = Data.ServiceOfferings.Where(s => s.IsActive);
            MockDataTableBinder.Expect(m => m.Bind(Arg<IQueryable<ServiceOffering>>.Matches(s => s.Where(so => so.IsActive).SequenceEqual(expectedQuery)), Arg.Is(dataTable), Arg.Is(requestModel))).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenNullUser_WhenSetFavoriteState_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.SetFavoriteState(null, 1, false));
        }

        [TestMethod]
        public void GivenValidOfferingId_WhenSetFavoriteState_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("SetFavoriteServiceOffering", 1)).Return(permission);

            Target.SetFavoriteState(User, 1, true);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidOfferingId_WhenSetFavoriteState_ThenSucceed()
        {
            PermissionFactory.Current.Expect(m => m.Create("SetFavoriteServiceOffering", 1)).Return(MockRepository.GenerateMock<IPermission>());

            Target.SetFavoriteState(User, 1, true);
        }

        [TestMethod]
        public void GivenValidOfferingId_AndTrueState_WhenSetFavoriteState_ThenUserLinkAdded()
        {
            ServiceOffering toSetAsFavorite = Data.ServiceOfferings[1];
            PermissionFactory.Current.Expect(m => m.Create("SetFavoriteServiceOffering", toSetAsFavorite.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.SetFavoriteState(User, toSetAsFavorite.Id, true);

            Repositories.MockServiceOfferingRepository.AssertWasCalled(m => m.AddLink(toSetAsFavorite, User.Identity.User));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidOfferingId_AndFalseState_WhenSetFavoriteState_ThenUserLinkDeleted()
        {
            ServiceOffering toSetAsFavorite = Data.ServiceOfferings[1];
            PermissionFactory.Current.Expect(m => m.Create("SetFavoriteServiceOffering", toSetAsFavorite.Id)).Return(MockRepository.GenerateMock<IPermission>());

            Target.SetFavoriteState(User, toSetAsFavorite.Id, false);

            Repositories.MockServiceOfferingRepository.AssertWasCalled(m => m.DeleteLink(toSetAsFavorite, User.Identity.User));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidOfferingId_WhenSetFavoriteState_ThenThrowException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.SetFavoriteState(User, 2437582, false));
        }

        [TestMethod]
        public void GivenInactiveOfferingId_WhenSetFavoriteState_ThenThrowException()
        {
            int inactiveId = Data.ServiceOfferings.First(s => !s.IsActive).Id;

            Target.ExpectException<EntityNotFoundException>(() => Target.SetFavoriteState(User, inactiveId, false));
        }

        [TestMethod]
        public void GivenUserHasFavorites_WhenLoadFavorites_ThenReturnListOfFavorites()
        {
            Data.ServiceOfferings[1].UsersLinkingAsFavorite.Add(User.Identity.User);
            Data.ServiceOfferings[2].UsersLinkingAsFavorite.Add(User.Identity.User);

            IEnumerable<ServiceOffering> actual = Target.LoadFavorites(User);

            CollectionAssert.AreEqual(new ServiceOffering[] { Data.ServiceOfferings[1], Data.ServiceOfferings[2] }, actual.ToList());
        }

        [TestMethod]
        public void GivenUserHasNoFavorites_WhenLoadFavorites_ThenReturnEmptyListOfFavorites()
        {
            IEnumerable<ServiceOffering> actual = Target.LoadFavorites(User);

            CollectionAssert.AreEqual(new List<ServiceOffering>(), actual.ToList());
        }

        [TestMethod]
        public void GivenUserHasFavoritesThatAreInactive_WhenLoadFavorites_ThenReturnListOfFavoritesWithoutInactives()
        {
            Data.ServiceOfferings[1].UsersLinkingAsFavorite.Add(User.Identity.User);
            Data.ServiceOfferings.First(s => !s.IsActive).UsersLinkingAsFavorite.Add(User.Identity.User);

            IEnumerable<ServiceOffering> actual = Target.LoadFavorites(User);

            CollectionAssert.AreEqual(new ServiceOffering[] { Data.ServiceOfferings[1] }, actual.ToList());
        }

        [TestMethod]
        public void GivenInvalidServiceOfferingId_WhenCheckStudentAssignedOfferings_ThenThrowException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.CheckStudentAssignedOfferings(50000));
        }

        [TestMethod]
        public void GivenServiceOfferingAssociatedWithStudentAssignedOfferings_WhenCheckStudentOfferings_ThenThrowException()
        {
            Target.ExpectException<ValidationException>(() => Target.CheckStudentAssignedOfferings(1));
        }

        [TestMethod]
        public void GivenOnlyInactiveStudentAssignedOffering_WhenCheckStudentOfferings_ThenSucceed() 
        {
            Data.StudentAssignedOfferings.Clear();
            StudentAssignedOffering offering = new StudentAssignedOffering{ ServiceOfferingId = 1, IsActive = false };
            Data.ServiceOfferings[0].StudentAssignedOfferings.Add(offering);
            Data.StudentAssignedOfferings.Add(offering);

            Target.CheckStudentAssignedOfferings(1);
        }
    }
}
