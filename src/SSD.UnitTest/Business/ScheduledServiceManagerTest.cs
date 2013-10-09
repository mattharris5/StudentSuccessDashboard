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
    public class ScheduledServiceManagerTest : BaseManagerTest
    {
        private ScheduledServiceManager Target { get; set; }

        [TestInitialize]
        public void InitializeTest()
        {
            Target = new ScheduledServiceManager(Repositories.MockRepositoryContainer);
        }

        [TestMethod]
        public void GivenNullRepositoryContainer_WhenConstruct_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => new ScheduledServiceManager(null));
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenGenerateScheduleOfferingViewModel_ThenViewModelWithAllStudentIdsIsReturned()
        {
            var expected = Data.Students.Select(s => s.Id).ToList();
            var students = Data.Students.Where(s => expected.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, expected);

            Assert.IsNotNull(actual);
            CollectionAssert.AreEqual(expected, actual.SelectedStudents.ToList());
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenGenerateScheduleOfferingViewModel_ThenAttemptGrantAccess()
        {
            var studentIds = Data.Students.Select(s => s.Id);
            var students = Data.Students.Where(s => studentIds.Contains(s.Id));
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(permission);

            Target.GenerateScheduleOfferingViewModel(User, studentIds);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenGenerateScheduleOfferingViewModel_ThenViewModelHasExpectedStudentList()
        {
            List<int> expectedStudentList = new List<int> { 1, 2 };
            var students = Data.Students.Where(s => expectedStudentList.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, expectedStudentList);

            CollectionAssert.AreEqual(expectedStudentList, actual.SelectedStudents.ToList());
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenGenerateScheduleOfferingViewModel_ThenViewModelHasExpectedFilterLists()
        {
            var studentIds = new int[] { 1, 2 };
            var students = Data.Students.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, studentIds);

            CollectionAssert.AreEqual(Data.Categories.Select(c => c.Name).ToList(), actual.CategoryFilterList.ToList());
            CollectionAssert.AreEqual(Data.ServiceTypes.Where(s => s.IsActive).Select(c => c.Name).ToList(), actual.TypeFilterList.ToList());
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenGenerateScheduleOfferingViewModel_ThenViewModelHasExpectedFavorites()
        {
            int[] studentIds = new int[] { 1, 2 };
            var students = Data.Students.Where(s => studentIds.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            Data.ServiceOfferings[1].UsersLinkingAsFavorite.Add(User.Identity.User);
            Data.ServiceOfferings[2].UsersLinkingAsFavorite.Add(User.Identity.User);

            ScheduleServiceOfferingListOptionsModel actual = Target.GenerateScheduleOfferingViewModel(User, studentIds);

            CollectionAssert.AreEqual(new ServiceOffering[] { Data.ServiceOfferings[1], Data.ServiceOfferings[2] }, actual.Favorites.ToList());
        }

        [TestMethod]
        public void GivenInvalidServiceOfferingId_WhenGenerateCreateViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateCreateViewModel(3489));
        }

        [TestMethod]
        public void GivenInactiveServiceOfferingId_WhenGenerateCreateViewModel_ThenThrowEntityNotFound()
        {
            int inactiveId = Data.ServiceOfferings.First(s => !s.IsActive).Id;

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateCreateViewModel(inactiveId));
        }

        [TestMethod]
        public void GivenValidServiceOfferingId_WhenGenerateCreateViewModel_ThenViewModelHasServiceOfferingId()
        {
            int expected = 2;

            ServiceOfferingScheduleModel actual = Target.GenerateCreateViewModel(expected);

            Assert.AreEqual(expected, actual.ServiceOfferingId);
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenCreate_ThenNewStudentAssignedOfferingIsActive()
        {
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = new List<int> { 2, 4 }, ServiceOfferingId = 2 };
            List<StudentAssignedOffering> studentsAdded = new List<StudentAssignedOffering>();
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            Repositories.MockStudentAssignedOfferingRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<StudentAssignedOffering>(s => studentsAdded.Add(s)));

            Target.Create(User, viewModel);

            Assert.IsTrue(studentsAdded.All(s => s.IsActive == true));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenInvalidServiceOfferingSelected_WhenCreate_ThenThrowException()
        {
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { ServiceOfferingId = 1987897 };

            Target.ExpectException<EntityNotFoundException>(() => Target.Create(User, viewModel));
        }

        [TestMethod]
        public void GivenInactiveServiceOfferingSelected_WhenCreate_ThenThrowException()
        {
            int inactiveId = Data.ServiceOfferings.First(s => !s.IsActive).Id;
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = new List<int> { 2, 4 }, ServiceOfferingId = inactiveId };
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());

            Target.ExpectException<EntityNotFoundException>(() => Target.Create(User, viewModel));
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenCreate_ThenServiceOfferingAssignmentsAdded()
        {
            List<int> studentIds = new List<int> { 2, 4 };
            List<Student> expectedStudents = Data.Students.Where(s => studentIds.Contains(s.Id)).ToList();
            int expectedServiceOfferingId = 2;
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = studentIds, ServiceOfferingId = expectedServiceOfferingId };
            List<StudentAssignedOffering> studentsAdded = new List<StudentAssignedOffering>();
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            Repositories.MockStudentAssignedOfferingRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<StudentAssignedOffering>(s => studentsAdded.Add(s)));

            Target.Create(User, viewModel);

            Assert.IsTrue(studentsAdded.All(a => a.ServiceOfferingId == expectedServiceOfferingId));
            CollectionAssert.AreEqual(expectedStudents.Select(s => s.Id).ToList(), studentsAdded.Select(a => a.StudentId).ToList());
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenCreate_ThenAttemptGrantAccess()
        {
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = new List<int> { 2, 4 }, ServiceOfferingId = 2 };
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(permission);

            Target.Create(User, viewModel);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenCreate_ThenSucceed()
        {
            List<int> studentIds = new List<int> { 2, 4 };
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = studentIds, ServiceOfferingId = 2 };
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());

            Target.Create(User, viewModel);
        }

        [TestMethod]
        public void GivenValidStudentIds_WhenCreate_ThenAddedServiceOfferingAssignmentHasTimestamp()
        {
            List<int> studentIds = new List<int> { 2, 4 };

            var added = CreateAndEnsureCorrectStudentAssignmentCountAdded(studentIds);

            Assert.IsTrue(added.All(a => a.CreateTime.Ticks / TimeSpan.TicksPerSecond == DateTime.Now.Ticks / TimeSpan.TicksPerSecond));
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenGenerateEditViewModel_ThenViewModelReturnedWithAssignedOfferingState()
        {
            StudentAssignedOffering expectedState = Data.StudentAssignedOfferings[0];
            int expectedStudentId = expectedState.StudentId;
            PermissionFactory.Current.Expect(m => m.Create("EditScheduledOffering", expectedState)).Return(MockRepository.GenerateMock<IPermission>());

            StudentServiceOfferingScheduleModel actual = Target.GenerateEditViewModel(User, expectedState.Id);

            AssertModelState(expectedState, expectedStudentId, actual);
        }

        [TestMethod]
        public void GivenInvalidAssignedServiceOfferingId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, 2347895));
        }

        [TestMethod]
        public void GivenInactiveAssignedServiceOfferingId_WhenGenerateEditViewModel_ThenThrowEntityNotFound()
        {
            int inactiveId = Data.StudentAssignedOfferings.Single(a => !a.IsActive).Id;

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateEditViewModel(User, inactiveId));
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId__WhenGenerateEditViewModel_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering expectedState = Data.StudentAssignedOfferings[0];
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("EditScheduledOffering", expectedState)).Return(permission);

            Target.GenerateEditViewModel(User, 1);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenOnlyHaveOneStudentAssignedOffering_WhenEdit_ThenItIsUpdated()
        {
            StudentAssignedOffering toEdit = Data.StudentAssignedOfferings[4];
            StudentServiceOfferingScheduleModel viewModel = new StudentServiceOfferingScheduleModel
            {
                Id = toEdit.Id,
                EndDate = DateTime.Now
            };
            PermissionFactory.Current.Expect(m => m.Create("EditScheduledOffering", toEdit)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Repositories.MockStudentAssignedOfferingRepository.AssertWasCalled(a => a.Update(toEdit));
            Repositories.MockRepositoryContainer.AssertWasCalled(a => a.Save());
        }

        [TestMethod]
        public void GivenValidUser_WhenEdit_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering toEdit = Data.StudentAssignedOfferings[0];
            StudentServiceOfferingScheduleModel viewModel = new StudentServiceOfferingScheduleModel
            {
                Id = toEdit.Id
            };
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("EditScheduledOffering", toEdit)).Return(permission);

            Target.Edit(User, viewModel);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenEdit_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering toEdit = Data.StudentAssignedOfferings[0];
            StudentServiceOfferingScheduleModel viewModel = new StudentServiceOfferingScheduleModel
            {
                Id = toEdit.Id
            };
            PermissionFactory.Current.Expect(m => m.Create("EditScheduledOffering", toEdit)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Edit(User, viewModel);

            Assert.IsTrue(toEdit.LastModifyTime.Value.WithinTimeSpanOf(TimeSpan.FromSeconds(1), DateTime.Now));
            Assert.AreEqual(User.Identity.User, toEdit.LastModifyingUser);
        }

        [TestMethod]
        public void GivenInactiveId_WhenEdit_ThenThrowException()
        {
            int inactiveId = Data.StudentAssignedOfferings.Single(a => !a.IsActive).Id;
            StudentServiceOfferingScheduleModel viewModel = new StudentServiceOfferingScheduleModel
            {
                Id = inactiveId
            };

            Target.ExpectException<EntityNotFoundException>(() => Target.Edit(User, viewModel));
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenGenerateDeleteViewModel_ThenViewModelReturned()
        {
            StudentAssignedOffering expectedState = Data.StudentAssignedOfferings[0];
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", expectedState)).Return(MockRepository.GenerateMock<IPermission>());

            DeleteServiceOfferingScheduleModel actual = Target.GenerateDeleteViewModel(User, expectedState.Id);

            Assert.IsNotNull(actual);
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenGenerateDeleteViewModel_ThenViewModelStateMatches()
        {
            StudentAssignedOffering expectedState = Data.StudentAssignedOfferings[0];
            int expectedStudentId = expectedState.StudentId;
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", expectedState)).Return(MockRepository.GenerateMock<IPermission>());

            DeleteServiceOfferingScheduleModel actual = Target.GenerateDeleteViewModel(User, expectedState.Id);

            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedStudentId, actual.StudentId);
            Assert.AreEqual(expectedState.ServiceOffering.Name, actual.Name);
        }

        [TestMethod]
        public void GivenAnInvalidStudentAssignedOfferingId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(User, 786876));
        }

        [TestMethod]
        public void GivenAnInactiveStudentAssignedOfferingId_WhenGenerateDeleteViewModel_ThenThrowEntityNotFound()
        {
            int inactiveId = Data.StudentAssignedOfferings.Single(a => !a.IsActive).Id;

            Target.ExpectException<EntityNotFoundException>(() => Target.GenerateDeleteViewModel(User, inactiveId));
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenGenerateDeleteViewModel_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering expectedState = Data.StudentAssignedOfferings[0];
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", expectedState)).Return(permission);

            Target.GenerateDeleteViewModel(User, expectedState.Id);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_AndAssignedOfferingIsForMultipleStudents_WhenDelete_ThenStudentAssignedOfferingSetInactive()
        {
            StudentAssignedOffering toRemove = Data.StudentAssignedOfferings[0];
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", toRemove)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Delete(User, toRemove.Id);

            Assert.IsFalse(toRemove.IsActive);
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenDelete_ThenStudentAssignedOfferingIsUpdated_AndSaved()
        {
            StudentAssignedOffering toRemove = Data.StudentAssignedOfferings[0];
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", toRemove)).Return(MockRepository.GenerateMock<IPermission>());

            Target.Delete(User, toRemove.Id);

            Repositories.MockStudentAssignedOfferingRepository.AssertWasCalled(m => m.Update(toRemove));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
        }

        [TestMethod]
        public void GivenValidAssignedServiceOfferingId_WhenDelete_ThenAttemptGrantAccess()
        {
            StudentAssignedOffering toRemove = Data.StudentAssignedOfferings[0];
            IPermission permission = MockRepository.GenerateMock<IPermission>();
            PermissionFactory.Current.Expect(m => m.Create("DeleteScheduledOffering", toRemove)).Return(permission);

            Target.Delete(User, toRemove.Id);

            permission.AssertWasCalled(p => p.GrantAccess(User));
        }

        [TestMethod]
        public void GivenAnInvalidStudentAssignedOfferingId_WhenDelete_ThenThrowEntityNotFoundException()
        {
            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(User, 786876));
        }

        [TestMethod]
        public void GivenAnInactiveStudentAssignedOfferingId_WhenDelete_ThenThrowEntityNotFoundException()
        {
            int inactiveId = Data.StudentAssignedOfferings.Single(a => !a.IsActive).Id;

            Target.ExpectException<EntityNotFoundException>(() => Target.Delete(User, inactiveId));
        }

        [TestMethod]
        public void GivenNullUser_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(null, new ServiceOfferingScheduleModel()));
        }

        [TestMethod]
        public void GivenNullViewModel_WhenCreate_ThenThrowException()
        {
            Target.ExpectException<ArgumentNullException>(() => Target.Create(User, null));
        }

        private List<StudentAssignedOffering> CreateAndEnsureCorrectStudentAssignmentCountAdded(List<int> studentIds)
        {
            ServiceOfferingScheduleModel viewModel = new ServiceOfferingScheduleModel { SelectedStudents = studentIds, ServiceOfferingId = 2 };
            List<StudentAssignedOffering> added = new List<StudentAssignedOffering>();
            var students = Data.Students.Where(s => viewModel.SelectedStudents.Contains(s.Id));
            PermissionFactory.Current.Expect(m => m.Create(Arg.Is<string>("ScheduleOffering"), Arg<object[]>.Matches(paramsArg => AreStudentListEqual(paramsArg, students)))).Return(MockRepository.GenerateMock<IPermission>());
            Repositories.MockStudentAssignedOfferingRepository.Expect(m => m.Add(null)).IgnoreArguments().Do(new Action<StudentAssignedOffering>(a => added.Add(a)));

            Target.Create(User, viewModel);

            Assert.AreEqual(studentIds.Count, added.Count);
            return added;
        }

        private void AssertAssignedOffering(StudentServiceOfferingScheduleModel viewModel, List<StudentAssignedOffering> added)
        {
            var assignedOffering = added.Single();
            Repositories.MockStudentAssignedOfferingRepository.AssertWasNotCalled(m => m.Update(null), options => options.IgnoreArguments());
            Repositories.MockStudentAssignedOfferingRepository.AssertWasCalled(m => m.Add(assignedOffering));
            Repositories.MockRepositoryContainer.AssertWasCalled(m => m.Save());
            Assert.AreEqual(viewModel.EndDate, assignedOffering.EndDate);
            Assert.AreEqual(viewModel.Notes, assignedOffering.Notes);
            Assert.AreEqual(viewModel.StartDate, assignedOffering.StartDate);
        }

        private static void AssertAssignedOffering(StudentServiceOfferingScheduleModel expected, StudentAssignedOffering actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(0, actual.Id); //Since we are adding the Id will always be 0
            Assert.AreEqual(expected.EndDate, actual.EndDate);
            Assert.AreEqual(expected.Notes, actual.Notes);
            Assert.AreEqual(expected.StartDate, actual.StartDate);
        }

        private static void AssertModelState(StudentAssignedOffering expectedState, int expectedStudentId, StudentServiceOfferingScheduleModel actual)
        {
            Assert.AreEqual(expectedState.Id, actual.Id);
            Assert.AreEqual(expectedState.EndDate, actual.EndDate);
            Assert.AreEqual(expectedState.Notes, actual.Notes);
            Assert.AreEqual(expectedState.StartDate, actual.StartDate);
        }

        private bool AreStudentListEqual(object[] paramsArg, IEnumerable<Student> second)
        {
            IEnumerable<Student> first = (IEnumerable<Student>)paramsArg[0];
            return first.SequenceEqual(second);
        }
    }
}
