using Castle.Windsor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SSD.Data;
using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace SSD.Business
{
    [TestClass]
    public class ServiceRequestManagerTest
    {
        private WindsorContainer Container { get; set; }
        private EducationDataContext EducationContext { get; set; }
        private ServiceRequestManager Target { get; set; }
        private EducationSecurityPrincipal User { get; set; }

        [TestInitialize]
        public void TestInitialize()
        {
            EducationContext = new EducationDataContext();
            Container = AssemblySetup.CreateWindsorContainer(EducationContext);
            RepositoryContainer repositoryContainer = new RepositoryContainer(Container, EducationContext);
            Target = new ServiceRequestManager(repositoryContainer);
            User = new EducationSecurityPrincipal(new UserRepository(EducationContext).Items.Where(s => s.UserKey == "Bob").Include("UserRoles.Role").Single());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            if (Container != null)
            {
                Container.Dispose();
            }
            if (EducationContext != null)
            {
                EducationContext.Dispose();
            }
        }

        [TestMethod]
        public void GivenViewModelHasStudentId_AndStudentAssignedOffering_WhenPopulateViewModel_ThenAssignedOfferingContainsFullName()
        {
            int studentId = EducationContext.Students.Where(s => s.StudentAssignedOfferings.Any()).Select(s => s.Id).First();
            ServiceRequestModel viewModel = new ServiceRequestModel { StudentIds = new[] { studentId } };

            Target.PopulateViewModel(viewModel);

            using (EducationDataContext verificationContext = new EducationDataContext())
            {
                var expected = verificationContext.Students.Where(s => s.Id == studentId).Select(s => s.StudentAssignedOfferings.FirstOrDefault()).Select(a => new { Provider = a.ServiceOffering.Provider, Program = a.ServiceOffering.Program, ServiceType = a.ServiceOffering.ServiceType }).Single();

                StringAssert.Contains(viewModel.AssignedOfferings.First().Text, expected.Program.Name);
                StringAssert.Contains(viewModel.AssignedOfferings.First().Text, expected.Provider.Name);
                StringAssert.Contains(viewModel.AssignedOfferings.First().Text, expected.ServiceType.Name);
            }
        }

        [TestMethod]
        public void GivenValidViewModel_AndUserIsCreator_WhenEdit_ThenEditDoesNotThrowException()
        {
            ServiceRequest newRequestToEdit = CreateServiceRequestInDatabase(User.Identity.User.Id, 2);
            ServiceRequestModel viewModel = new ServiceRequestModel { Id = newRequestToEdit.Id, SelectedPriorityId = newRequestToEdit.PriorityId, SelectedServiceTypeId = newRequestToEdit.ServiceTypeId, SelectedSubjectId = newRequestToEdit.SubjectId, StudentIds = new int[] { newRequestToEdit.StudentId }, SelectedStatusId = 1, FulfillmentNotes = "Test Notes" };
            
            Target.Edit(User, viewModel);
        }

        [TestMethod]
        public void GivenValidViewModel_AndUserIsDataAdmin_WhenEdit_ThenEditDoesNotThrowException()
        {
            ServiceRequest newRequestToEdit = CreateServiceRequestInDatabase(2, 2);
            ServiceRequestModel viewModel = new ServiceRequestModel { Id = newRequestToEdit.Id, SelectedPriorityId = newRequestToEdit.PriorityId, SelectedServiceTypeId = newRequestToEdit.ServiceTypeId, SelectedSubjectId = newRequestToEdit.SubjectId, StudentIds = new int[] { newRequestToEdit.StudentId }, FulfillmentNotes = "blah" };
            
            Target.Edit(User, viewModel);
        }

        [TestMethod]
        public void GivenValidViewModel_AndUserIsNotAdministrator_AndUserIsNotCreator_WhenEdit_ThenThrowEntityAccessUnauthorizedException()
        {
            User nonAdminUserEntity = EducationContext.Users.Where(u => u.UserKey == "Fred").Include("UserRoles.Role").Single();
            EducationSecurityPrincipal nonAdminUser = new EducationSecurityPrincipal(nonAdminUserEntity);
            ServiceRequest newRequestToEdit = CreateServiceRequestInDatabase(2, 3);
            ServiceRequestModel viewModel = new ServiceRequestModel { Id = newRequestToEdit.Id, SelectedPriorityId = newRequestToEdit.PriorityId, SelectedServiceTypeId = newRequestToEdit.ServiceTypeId, SelectedSubjectId = newRequestToEdit.SubjectId, StudentIds = new int[] { newRequestToEdit.StudentId } };

            Target.ExpectException<EntityAccessUnauthorizedException>(() => Target.Edit(nonAdminUser, viewModel));
        }

        [TestMethod]
        public void GivenValidViewModel_AndUserIsNotAdministrator_AndUserIsNotCreator_WhenDelete_ThenThrowEntityAccessUnauthorizedException()
        {
            User nonAdminUserEntity = EducationContext.Users.Where(u => u.UserKey == "Fred").Include("UserRoles.Role").Single();
            EducationSecurityPrincipal nonAdminUser = new EducationSecurityPrincipal(nonAdminUserEntity);
            ServiceRequest newRequestToDelete = CreateServiceRequestInDatabase(2, 3);
            var viewModel = new ServiceRequestModel { Id = newRequestToDelete.Id, SelectedPriorityId = newRequestToDelete.PriorityId, SelectedServiceTypeId = newRequestToDelete.ServiceTypeId, SelectedSubjectId = newRequestToDelete.SubjectId, StudentIds = new int[] { newRequestToDelete.StudentId } };
            
            Target.ExpectException<EntityAccessUnauthorizedException>(() => Target.Delete(nonAdminUser, viewModel.Id));
        }

        [TestMethod]
        public void GivenValidViewModel_WhenDelete_ThenAssociatedServiceRequestFulfillmentsAreAlsoDeleted()
        {
            ServiceRequest newRequestToDelete = CreateServiceRequestInDatabase(2, 3);
            ServiceRequestFulfillment fulfillment = EducationContext.ServiceRequestFulfillments.Where(s => s.ServiceRequestId == newRequestToDelete.Id).FirstOrDefault();
            
            Target.Delete(User, newRequestToDelete.Id);

            Assert.IsTrue(EducationContext.ServiceRequestFulfillments.Where(s => s.Id == fulfillment.Id).Count() == 0);
        }

        [TestMethod]
        public void GivenServiceRequestWasModified_WhenGenerateEditViewModel_ThenViewModelContainsAuditData()
        {
            ServiceRequest toEdit = EducationContext.ServiceRequests.First(r => r.LastModifyingUser != null);

            ServiceRequestModel actual = Target.GenerateEditViewModel(User, toEdit.Id);

            Assert.IsNotNull(actual.Audit.CreatedBy);
            Assert.AreNotEqual(DateTime.MinValue, actual.Audit.CreatedBy);
            Assert.IsNotNull(actual.Audit.LastModifiedBy);
            Assert.IsTrue(actual.Audit.LastModifyTime.HasValue);
        }

        private static ServiceRequest CreateServiceRequestInDatabase(int creatingUserId, int priorityId)
        {
            using (EducationDataContext context = new EducationDataContext())
            {
                ServiceRequestFulfillment newServiceRequestFulfillment = new ServiceRequestFulfillment { FulfillmentStatusId = 1, Notes = "Test", CreatingUserId = 1 };
                ServiceRequest newRequestToEdit = new ServiceRequest { CreatingUserId = creatingUserId, PriorityId = priorityId, ServiceTypeId = 1, StudentId = 6, SubjectId = 2, FulfillmentDetails = new List<ServiceRequestFulfillment> { newServiceRequestFulfillment } };
                context.ServiceRequests.Add(newRequestToEdit);
                context.SaveChanges();
                return newRequestToEdit;
            }
        }
    }
}
