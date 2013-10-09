using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;
using SSD.Domain;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SSD.Business
{
    [TestClass]
    public class ServiceRequestManagerTest : BaseManagerTest
    {
        private ServiceRequestManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ServiceRequestManager(Repositories.MockRepositoryContainer);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ServiceRequestManager(null));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenGenerateDeleteViewModel_ThenGenerateViewModel()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("DeleteRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());

            ServiceRequestModel actual = Target.GenerateDeleteViewModel(User, 1);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenInvalidServiceRequestId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(User, 789669));
        }

        [TestMethod]
        public void GivenValidServiceRequestId_WhenGenerateDeleteViewModel_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("DeleteRequest", mockRequest)).Return(permission);

            Target.GenerateDeleteViewModel(User, 1);

            permission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidServiceRequestId_AndPermissionGrantsAccess_WhenGenerateDeleteViewModel_ThenViewModelIsCorrect()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("DeleteRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());
            var expected = Data.ServiceRequests[0];

            var actual = Target.GenerateDeleteViewModel(User, 1);

            Assert.AreEqual(1, actual.Id);
            Assert.IsTrue(actual.Name.Contains(expected.ServiceType.Name) && actual.Name.Contains(expected.Subject.Name));
        }

        [TestMethod]
        public void GivenNullUser_WhenDelete_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Delete(null, 1));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenDelete_ThenSucceed()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("DeleteRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Delete(User, 1);
        }

        [TestMethod]
        public void GivenValidId_WhenDelete_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("DeleteRequest", mockRequest)).Return(permission);

            Target.Delete(User, 1);

            permission.AssertWasCalled(m => m.GrantAccess(User));
        }

        [TestMethod]
        public void GivenInvalidId_WhenDelete_ThenThrowException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(User, 9839797));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenGenerateEditViewModel_ThenViewModelListsPopulated()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());

            var actual = Target.GenerateEditViewModel(User, 1);

            AssertSelectLists(actual);
        }

        [TestMethod]
        public void GivenValidId_WhenGenerateEditViewModel_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(permission);

            var actual = Target.GenerateEditViewModel(User, 1);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenInvalidServiceRequestId_WhenEdit_ThenAnEntityNotFoundExceptionIsThrown()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(User, new ServiceRequestModel { Id = 787979 }));
        }

        [TestMethod]
        public void GivenNullUser_WhenEdit_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Edit(User, null));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenEdit_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Edit(null, new ServiceRequestModel()));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenEdit_ThenItIsCorrectlyUpdatedInTheRepository()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());
            var expected = Data.ServiceRequests[0];
            expected.FulfillmentDetails = Data.ServiceRequestFulfillments;

            Target.Edit(User, new ServiceRequestModel { Id = 1, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 } });

            AssertSuccessfulUpdate(expected);
        }

        [TestMethod]
        public void GivenStatusChanged_WhenEdit_ThenNewFulfillmentDetailIsCreated()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());
            var expected = Data.ServiceRequests[0].FulfillmentDetails.Count() + 1;
            ServiceRequest actual = null;
            Repositories.MockServiceRequestRepository.Expect(m => m.Update(mockRequest)).Do(new Action<ServiceRequest>(s => actual = s));

            Target.Edit(User, new ServiceRequestModel { Id = 1, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 }, SelectedStatusId = 1, FulfillmentNotes = "test"});

            Assert.AreEqual(expected, actual.FulfillmentDetails.Count());
        }

        [TestMethod]
        public void GivenOnlyOfferingHasChanged_WhenEdit_ThenCurrentFulfillmentDetailIsUpdated()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());
            var request = Data.ServiceRequests[0];
            request.FulfillmentDetails = Data.ServiceRequestFulfillments;

            Target.Edit(User, new ServiceRequestModel { Id = 1, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 }, SelectedAssignedOfferingId = 2, SelectedStatusId = 2 });

            Repositories.MockServiceRequestFulfillmentRepository.AssertWasCalled(m => m.Update(request.FulfillmentDetails.OrderByDescending(e => e.CreateTime).First()));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void WhenEdit_ThenModifyAuditDataSet()
        {
            int requestId = 1;
            var viewModel = new ServiceRequestModel { Id = requestId, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 }, SelectedAssignedOfferingId = 2, SelectedStatusId = 2 };
            ServiceRequest request = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == requestId).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", request)).Return(MockRepository.GenerateMock<IPermission>());
            request.FulfillmentDetails = Data.ServiceRequestFulfillments;

            Target.Edit(User, viewModel);

            Assert.AreEqual(User.Identity.User, request.LastModifyingUser);
            Assert.IsTrue(request.LastModifyTime.Value.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now));
        }

        [TestMethod]
        public void GivenNeitherStatusOrOfferingHasChanged_WhenEdit_ThenTheFulfillmentNotUpdated()
        {
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(MockRepository.GenerateMock<IPermission>());
            var request = Data.ServiceRequests[0];
            request.FulfillmentDetails = Data.ServiceRequestFulfillments;

            Target.Edit(User, new ServiceRequestModel { Id = 1, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 }, SelectedStatusId = 2 });

            Repositories.MockServiceRequestFulfillmentRepository.AssertWasNotCalled(m => m.Update(request.FulfillmentDetails.OrderByDescending(e => e.CreateTime).First()));
        }

        [TestMethod]
        public void GivenValidId_WhenEdit_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            var mockRequest = Repositories.MockServiceRequestRepository.Items.Where(s => s.Id == 1).SingleOrDefault();
            PermissionFactory.Current.Expect(m => m.Create("EditRequest", mockRequest)).Return(permission);

            Target.Edit(User, new ServiceRequestModel { Id = 1, SelectedPriorityId = Data.Priorities[3].Id, StudentIds = new List<int> { 1 } });

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelHasPopulatedPriorityList()
        {
            ServiceRequestModel actual = Target.GenerateCreateViewModel();

            Assert.IsNotNull(actual.Priorities);
            CollectionAssert.AreEqual(Data.Priorities, actual.Priorities.Items.Cast<Priority>().ToList());
            Assert.AreEqual("Id", actual.Priorities.DataValueField);
            Assert.AreEqual("Name", actual.Priorities.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelHasPopulatedServiceTypeList()
        {
            ServiceRequestModel actual = Target.GenerateCreateViewModel();

            Assert.IsNotNull(actual.ServiceTypes);
            CollectionAssert.AreEqual(Data.ServiceTypes.Where(s => s.IsActive).ToList(), actual.ServiceTypes.Items.Cast<ServiceType>().ToList());
            Assert.AreEqual("Id", actual.ServiceTypes.DataValueField);
            Assert.AreEqual("Name", actual.ServiceTypes.DataTextField);
        }

        [TestMethod]
        public void WhenGenerateCreateViewModel_ThenViewModelHasPopulatedSubjectList()
        {
            ServiceRequestModel actual = Target.GenerateCreateViewModel();

            Assert.IsNotNull(actual.Subjects);
            CollectionAssert.AreEqual(Data.Subjects, actual.Subjects.Items.Cast<Subject>().ToList());
            Assert.AreEqual("Id", actual.Subjects.DataValueField);
            Assert.AreEqual("Name", actual.Subjects.DataTextField);
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(User, null));
        }

        [TestMethod]
        public void GivenANullUser_WhenCreate_ThenAnArgumentNullExceptionIsThrown()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null, new ServiceRequestModel()));
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenCreate_ThenTheRequestIsSaved()
        {
            List<int> studentIds = new List<int> { 1, 2, 3 };
            var students = Repositories.MockStudentRepository.Items.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("CreateServiceRequest"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            Target.Create(User, new ServiceRequestModel
            {
                SelectedSubjectId = 1,
                SelectedPriorityId = 1,
                SelectedServiceTypeId = 1,
                StudentIds = studentIds
            });

            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_WhenCreate_ThenNewRequestReferencesCreatingUser()
        {
            int expected = 323;
            List<int> studentIds = new List<int> { 1 };
            var students = Repositories.MockStudentRepository.Items.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("CreateServiceRequest"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            ServiceRequest actualAdded = null;
            ServiceRequestModel toCreate = new ServiceRequestModel { StudentIds = studentIds };
            Repositories.MockServiceRequestRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<ServiceRequest>(s => actualAdded = s));
            User.Identity.User.Id = expected;

            Target.Create(User, toCreate);

            Assert.IsNotNull(actualAdded);
            Assert.AreEqual(expected, actualAdded.CreatingUserId);
        }

        [TestMethod]
        public void GivenPermissionGrantsAccess_AndViewModelHasNotes_WhenCreate_ThenNewRequestHasNotes()
        {
            List<int> studentIds = new List<int> { 1 };
            var students = Repositories.MockStudentRepository.Items.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("CreateServiceRequest"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            string expected = "These are the general notes!";
            ServiceRequest actualAdded = null;
            ServiceRequestModel toCreate = new ServiceRequestModel { StudentIds = studentIds, Notes = expected };
            Repositories.MockServiceRequestRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<ServiceRequest>(s => actualAdded = s));

            Target.Create(User, toCreate);

            Assert.IsNotNull(actualAdded);
            Assert.AreEqual(expected, actualAdded.Notes);
        }

        [TestMethod]
        public void GivenValidViewModel_WhenCreate_ThenNewRequestHasCurrentTimestamp()
        {
            List<int> studentIds = new List<int> { 1, 2, 3 };
            var students = Repositories.MockStudentRepository.Items.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("CreateServiceRequest"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            ServiceRequest actualAdded = null;
            ServiceRequestModel toCreate = new ServiceRequestModel { StudentIds = studentIds };
            Repositories.MockServiceRequestRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<ServiceRequest>(s => actualAdded = s));

            Target.Create(User, toCreate);

            Assert.IsNotNull(actualAdded);
            Assert.AreEqual(DateTime.Now.Ticks / TimeSpan.TicksPerSecond, actualAdded.CreateTime.Ticks / TimeSpan.TicksPerSecond);
        }

        [TestMethod]
        public void GivenValidIds_WhenCreate_ThenAttemptGrantAccess()
        {
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            List<int> studentIds = new List<int> { 1 };
            var students = Repositories.MockStudentRepository.Items.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("CreateServiceRequest"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(permission);
            ServiceRequestModel toCreate = new ServiceRequestModel { StudentIds = studentIds };
            
            Target.Create(User, toCreate);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenNewEmptyViewModel_WhenPopulateViewModel_ThenNonePrioritySubjectAndStatusOptionsSelectedByDefaultInList()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel();
            viewModel.SelectedAssignedOfferingId = 1;
            viewModel.StudentIds = new List<int> { 1 };
            
            Target.PopulateViewModel(viewModel);

            Assert.AreEqual(1, viewModel.Subjects.SelectedValue);
            Assert.AreEqual(1, viewModel.Priorities.SelectedValue);
            Assert.AreEqual(1, viewModel.AssignedOfferings.SelectedValue);
            Assert.AreEqual(1, viewModel.Statuses.SelectedValue);
        }

        [TestMethod]
        public void GivenNewEmptyViewModel_WhenPopulateViewModel_ThenNonePriorityAndSubjectOptionsSelectedByDefault()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel();
            viewModel.SelectedStatusId = 1;
            viewModel.StudentIds = new List<int> { 1 };

            Target.PopulateViewModel(viewModel);

            Assert.AreEqual(1, viewModel.SelectedSubjectId);
            Assert.AreEqual(1, viewModel.SelectedPriorityId);
        }

        [TestMethod]
        public void GivenViewModel_WhenPopulateViewModel_ThenOnlyActiveServiceTypesPopulated()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel();
            viewModel.SelectedStatusId = 1;
            viewModel.StudentIds = new List<int> { 1 };

            Target.PopulateViewModel(viewModel);

            Assert.IsFalse(viewModel.ServiceTypes.Items.Cast<ServiceType>().Where(s => !s.IsActive).Any());
        }

        [TestMethod]
        public void GivenServiceRequestHasInactiveServiceType_WhenPopulateViewModel_ThenServiceTypeShows()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel { SelectedServiceTypeId = 6 };
            viewModel.SelectedStatusId = 1;
            viewModel.StudentIds = new List<int> { 1 };

            Target.PopulateViewModel(viewModel);

            Assert.IsTrue(viewModel.ServiceTypes.Items.Cast<ServiceType>().Any(s => s.Id == 6));
        }

        [TestMethod]
        public void GivenViewModelWithStudentInfo_WhenPopulateViewModel_ThenItHasActiveAssignedOfferings()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel();
            viewModel.SelectedStatusId = 1;
            viewModel.StudentIds = new List<int> { 3 };
            var deactivated = Data.StudentAssignedOfferings.Where(s => !s.IsActive).Select(s => s.Id);

            Target.PopulateViewModel(viewModel);
            var ids = viewModel.AssignedOfferings.Select(a => int.Parse(a.Value));

            Assert.IsFalse(deactivated.Intersect(ids).Any());
        }

        [TestMethod]
        public void GivenViewModelWithStudentInfo_AndInactiveAssignedOfferingSelected_WhenPopulateViewModel_ThenItHasActiveAssignedOfferings()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel { SelectedAssignedOfferingId = 6 };
            viewModel.SelectedStatusId = 1;
            viewModel.StudentIds = new List<int> { 3 };
            var deactivated = Data.StudentAssignedOfferings.Single(s => s.Id == 6);

            Target.PopulateViewModel(viewModel);
            var ids = viewModel.AssignedOfferings.Select(a => int.Parse(a.Value));

            Assert.IsTrue(ids.Contains(deactivated.Id));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenPopulateViewModel_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.PopulateViewModel(null));
        }

        [TestMethod]
        public void GivenInValidServiceRequestId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 677688));
        }

        private void AssertSelectLists(ServiceRequestModel actual)
        {
            CollectionAssert.AreEqual(Data.Priorities, actual.Priorities.Items.Cast<Priority>().ToList());
            CollectionAssert.AreEqual(Data.ServiceTypes.Where(s => s.IsActive).ToList(), actual.ServiceTypes.Items.Cast<ServiceType>().ToList());
            CollectionAssert.AreEqual(Data.Subjects, actual.Subjects.Items.Cast<Subject>().ToList());
        }

        private void AssertSuccessfulUpdate(ServiceRequest expected)
        {
            Repositories.MockServiceRequestRepository.AssertWasCalled(m => m.Update(expected));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        private bool AreStudentListEqual(object[] paramsArg, IEnumerable<Student> second)
        {
            IEnumerable<Student> first = (IEnumerable<Student>)paramsArg[0];
            return first.SequenceEqual(second);
        }
    }
}
