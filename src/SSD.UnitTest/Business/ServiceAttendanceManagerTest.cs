using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ServiceAttendanceManagerTest : BaseManagerTest
    {
        private ServiceAttendanceManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceAttendanceManager(Repositories.MockRepositoryContainer, MockDataTableBinder);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ServiceAttendanceManager(null, MockDataTableBinder));
        }

        [TestMethod]
        public void GivenNullDataTableBinder_WhenConstruct_ThenThrowException()
        {
            TestExtensions.ExpectException<ArgumentNullException>(() => new ServiceAttendanceManager(Repositories.MockRepositoryContainer, null));
        }

        [TestMethod]
        public void GivenValidAssignedOfferingId_WhenGenerateCreateViewModel_ThenModelIsReturned()
        {
            StudentAssignedOffering addTo = Data.StudentAssignedOfferings.Single(a => a.Id == 1);
            PermissionFactory.Current.Expect(m => m.Create("CreateServiceAttendance", addTo)).Return(MockRepository.GenerateMock<IPermission>());

            var actual = Target.GenerateCreateViewModel(User, 1) as ServiceAttendanceModel;

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidAssignedOfferingId_WhenGenerateCreateViewModel_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering addTo = Data.StudentAssignedOfferings.Single(a => a.Id == 1);
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("CreateServiceAttendance", addTo)).Return(permission);

            Target.GenerateCreateViewModel(User, 1);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenInvalidAssignedOfferingId_WhenGenerateCreateViewModel_ThenThrowException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateCreateViewModel(User, 1876786));
        }

        [TestMethod]
        public void GivenBinderCreatesResult_WhenGenerateDataTableResultViewModel_ThenReturnBindResult()
        {
            DataTableResultModel expected = new DataTableResultModel();
            DataTableRequestModel requestModel = new DataTableRequestModel();
            IClientDataTable<ServiceAttendance> dataTable = MockRepository.GenerateMock<IClientDataTable<ServiceAttendance>>();
            MockDataTableBinder.Expect(m => m.Bind(Repositories.MockServiceAttendanceRepository.Items, dataTable, requestModel)).Return(expected);

            DataTableResultModel actual = Target.GenerateDataTableResultViewModel(requestModel, dataTable);

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GivenValidServiceAttendanceId_WhenGenerateEditViewModel_ThenAViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("EditServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceAttendanceModel actual = Target.GenerateEditViewModel(User, 1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidServiceAttendanceId_WhenGenerateEditViewModel_ThenViewModelStateMatchesEntity()
        {
            ServiceAttendance expectedState = Data.ServiceAttendances.Single(a => a.Id == 1);
            PermissionFactory.Current.Expect(m => m.Create("EditServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceAttendanceModel actual = Target.GenerateEditViewModel(User, 1);

            AssertState(expectedState, actual);
        }

        [TestMethod]
        public void GivenInvalidServiceAttendanceId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 0));
        }

        [TestMethod]
        public void GivenValidServiceAttendanceId_WhenGenerateDeleteViewModel_ThenAViewModelIsReturned()
        {
            PermissionFactory.Current.Expect(m => m.Create("DeleteServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceAttendanceModel actual = Target.GenerateDeleteViewModel(User, 1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidServiceAttendanceId_WhenGenerateDeleteViewModel_ThenViewModelStateMatchesEntity()
        {
            ServiceAttendance expectedState = Data.ServiceAttendances.Single(a => a.Id == 1);
            PermissionFactory.Current.Expect(m => m.Create("DeleteServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceAttendanceModel actual = Target.GenerateDeleteViewModel(User, 1);

            AssertState(expectedState, actual);
        }

        [TestMethod]
        public void GivenInvalidServiceAttendanceId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(User, 0));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenCreate_ThenThrowException()
        {
            ServiceAttendanceModel viewModel = new ServiceAttendanceModel();

            Target.ExpectException<ArgumentNullException>(() => Target.Create(viewModel, null));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenChangesSaved()
        {
            ServiceAttendanceModel viewModel = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, DateAttended = DateTime.Now };
            PermissionFactory.Current.Expect(m => m.Create("CreateServiceAttendance", Data.StudentAssignedOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());

            Target.Create(viewModel, User);

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenAddedServiceAttendanceHasStateFromViewModel()
        {
            ServiceAttendanceModel expectedState = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, DateAttended = DateTime.Now.AddDays(1), SelectedSubjectId = 1, Duration = 100, Notes = "Test Notes" };
            PermissionFactory.Current.Expect(m => m.Create("CreateServiceAttendance", Data.StudentAssignedOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());

            Target.Create(expectedState, User);

            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Add(Arg<ServiceAttendance>.Matches(a => AssertPropertiesMatch(expectedState, a))));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenAddedServiceAttendanceHasCreationAuditData()
        {
            ServiceAttendanceModel viewModel = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, DateAttended = DateTime.Now.AddDays(1), SelectedSubjectId = 1, Duration = 100, Notes = "Test Notes" };
            PermissionFactory.Current.Expect(m => m.Create("CreateServiceAttendance", Data.StudentAssignedOfferings[0])).Return(MockRepository.GenerateMock<IPermission>());

            Target.Create(viewModel, User);

            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Add(Arg<ServiceAttendance>.Matches(a => a.CreateTime.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now) && a.CreatingUser == User.Identity.User)));
        }

        [TestMethod]
        public void GivenInvalidEntityId_WhenEdit_ThenThrowEntityNotFoundException()
        {
            var viewModel = new ServiceAttendanceModel { Id = 0 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(viewModel, User));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenServiceAttendanceEdited_AndSaved()
        {
            ServiceAttendance expected = Repositories.MockServiceAttendanceRepository.Items.Where(s => s.Id == 1).Single();
            var viewModel = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, SelectedSubjectId = 1 };
            PermissionFactory.Current.Expect(m => m.Create("EditServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(viewModel, User);

            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenModifyAuditPropertiesSet()
        {
            ServiceAttendance toEdit = Repositories.MockServiceAttendanceRepository.Items.Where(s => s.Id == 1).Single();
            var viewModel = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, SelectedSubjectId = 1 };
            PermissionFactory.Current.Expect(m => m.Create("EditServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(viewModel, User);

            Assert.AreEqual(User.Identity.User, toEdit.LastModifyingUser);
            Assert.IsTrue(toEdit.LastModifyTime.Value.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenServiceAttendanceHasStateFromViewModel()
        {
            ServiceAttendance actualState = Repositories.MockServiceAttendanceRepository.Items.Where(s => s.Id == 1).Single();
            var expectedState = new ServiceAttendanceModel { Id = 1, StudentAssignedOfferingId = 1, SelectedSubjectId = 1, DateAttended = DateTime.Now.AddDays(1), Duration = 100, Notes = "Unit Test Notes" };
            PermissionFactory.Current.Expect(m => m.Create("EditServiceAttendance", Data.ServiceAttendances[0].StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(expectedState, User);

            Assert.AreEqual(expectedState.SelectedSubjectId, actualState.SubjectId);
            Assert.AreEqual(expectedState.DateAttended, actualState.DateAttended);
            Assert.AreEqual(expectedState.Duration, actualState.Duration);
            Assert.AreEqual(expectedState.Notes, actualState.Notes);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenEdit_ThenArgumentNullExceptionThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Edit(null, User));
        }

        [TestMethod]
        public void GivenNullUser_WhenEdit_ThenArgumentNullExceptionThrown()
        {
            ServiceAttendanceModel viewModel = new ServiceAttendanceModel();

            Target.ExpectException<ArgumentNullException>(() => Target.Edit(viewModel, null));
        }

        [TestMethod]
        public void GivenPostedServiceAttendanceDoesNotExist_WhenDelete_ThenHttpNotFoundResult()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(0, User));
        }

        [TestMethod]
        public void WhenDelete_ThenItIsDeletedFromTheRepository()
        {
            var expected = Data.ServiceAttendances[0];
            PermissionFactory.Current.Expect(m => m.Create("DeleteServiceAttendance", expected.StudentAssignedOffering)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Delete(expected.Id, User);

            Repositories.MockServiceAttendanceRepository.AssertWasCalled(m => m.Remove(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        private static bool AssertPropertiesMatch(ServiceAttendanceModel expectedState, ServiceAttendance actualState)
        {
            Assert.IsNotNull(actualState);
            Assert.AreEqual(expectedState.DateAttended, actualState.DateAttended);
            Assert.AreEqual(expectedState.SelectedSubjectId, actualState.SubjectId);
            Assert.AreEqual(expectedState.Duration, actualState.Duration);
            Assert.AreEqual(expectedState.Notes, actualState.Notes);
            return true;
        }

        private static void AssertState(ServiceAttendance expectedState, ServiceAttendanceModel actual)
        {
            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedState.Notes, actual.Notes);
            Assert.AreEqual(expectedState.StudentAssignedOfferingId, actual.StudentAssignedOfferingId);
            Assert.AreEqual(expectedState.SubjectId, actual.SelectedSubjectId);
        }
    }
}
