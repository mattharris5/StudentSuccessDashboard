using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    public class ServiceRequestManager : IServiceRequestManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IServiceRequestRepository ServiceRequestRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private ISubjectRepository SubjectRepository { get; set; }
        private IPriorityRepository PriorityRepository { get; set; }
        private IStudentRepository StudentRepository { get; set; }
        private IFulfillmentStatusRepository FulfillmentStatusRepository { get; set; }
        private IServiceRequestFulfillmentRepository ServiceRequestFulfillmentRepository { get; set; }

        public ServiceRequestManager(IRepositoryContainer repositories)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            RepositoryContainer = repositories;
            ServiceRequestRepository = repositories.Obtain<IServiceRequestRepository>();
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            SubjectRepository = repositories.Obtain<ISubjectRepository>();
            PriorityRepository = repositories.Obtain<IPriorityRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            FulfillmentStatusRepository = repositories.Obtain<IFulfillmentStatusRepository>();
            ServiceRequestFulfillmentRepository = repositories.Obtain<IServiceRequestFulfillmentRepository>();
        }

        public void PopulateViewModel(ServiceRequestModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            viewModel.Priorities = new SelectList(PriorityRepository.Items.OrderBy(p => p.Id), "Id", "Name", viewModel.SelectedPriorityId);
            viewModel.Subjects = new SelectList(LookupHelper.LoadSubjectList(SubjectRepository), "Id", "Name", viewModel.SelectedSubjectId);
            viewModel.ServiceTypes = new SelectList(ServiceTypeRepository.Items.Where(s => s.IsActive || viewModel.SelectedServiceTypeId == s.Id), "Id", "Name", viewModel.SelectedServiceTypeId);
            Student selectedStudent = null;
            if (viewModel.StudentIds != null)
            {
                selectedStudent = StudentRepository.Items.
                                                    Include("StudentAssignedOfferings.ServiceOffering.Provider").
                                                    Include("StudentAssignedOfferings.ServiceOffering.ServiceType").
                                                    Include("StudentAssignedOfferings.ServiceOffering.Program").
                                                    FirstOrDefault(s => s.Id == viewModel.StudentIds.FirstOrDefault());
            }
            List<StudentAssignedOffering> offerings = new List<StudentAssignedOffering>();
            if (selectedStudent != null)
            {
                viewModel.AssignedOfferings = new SelectList((from o in selectedStudent.StudentAssignedOfferings.Where(s => s.IsActive || (viewModel.SelectedAssignedOfferingId != null && s.Id == viewModel.SelectedAssignedOfferingId)).ToList()
                                                              select new
                                                              {
                                                                  Id = o.Id,
                                                                  Name = o.ServiceOffering.Name
                                                              }), "Id", "Name", viewModel.SelectedAssignedOfferingId);
            }
            else
            {
                viewModel.AssignedOfferings = new SelectList(Enumerable.Empty<StudentAssignedOffering>());
            }
            viewModel.Statuses = new SelectList(FulfillmentStatusRepository.Items, "Id", "Name", viewModel.SelectedStatusId);
        }

        public ServiceRequestModel GenerateEditViewModel(EducationSecurityPrincipal user, int requestId)
        {
            var serviceRequest = ServiceRequestRepository.Items.
                                                          Include(s => s.Priority).
                                                          Include(s => s.Subject).
                                                          Include(s => s.ServiceType).
                                                          Include(s => s.Student.ApprovedProviders).
                                                          Include(s => s.Student.School.UserRoles).
                                                          Include("Student.StudentAssignedOfferings.ServiceOffering").
                                                          Include("Student.StudentAssignedOfferings.ServiceOffering.Provider").
                                                          Include(s => s.FulfillmentDetails).
                                                          Include(s => s.CreatingUser).
                                                          Include(s => s.LastModifyingUser).
                                                          SingleOrDefault(s => s.Id == requestId);
            if (serviceRequest == null)
            {
                throw new EntityNotFoundException("Specified Service Request does not exist.");
            }
            IPermission permission = PermissionFactory.Current.Create("EditRequest", serviceRequest);
            permission.GrantAccess(user);
            ServiceRequestModel viewModel = new ServiceRequestModel();
            viewModel.CopyFrom(serviceRequest);
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public ServiceRequestModel GenerateCreateViewModel()
        {
            ServiceRequestModel viewModel = new ServiceRequestModel();
            PopulateViewModel(viewModel);
            viewModel.StudentIds = new List<int>();
            return viewModel;
        }

        public ServiceRequestModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int requestId)
        {
            var serviceRequest = ServiceRequestRepository.Items.
                                                          Include(s => s.ServiceType).
                                                          Include(s => s.Subject).
                                                          Include(s => s.Student.ApprovedProviders).
                                                          Include(s => s.Student.School.UserRoles).
                                                          SingleOrDefault(s => s.Id == requestId);
            if (serviceRequest == null)
            {
                throw new EntityNotFoundException("Service Request not found");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteRequest", serviceRequest);
            permission.GrantAccess(user);
            return new ServiceRequestModel
            {
                Id = serviceRequest.Id,
                Name = string.Format(CultureInfo.CurrentCulture, "{0} / {1}", serviceRequest.ServiceType.Name, serviceRequest.Subject.Name)
            };
        }

        public void Edit(EducationSecurityPrincipal user, ServiceRequestModel viewModel)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            var updatedServiceRequest = ServiceRequestRepository.Items.Include(s => s.ServiceType).
                                                                       Include(s => s.FulfillmentDetails).
                                                                       Include(s => s.Student.ApprovedProviders).
                                                                       Include(s => s.Student.School.UserRoles).
                                                                       Include("Student.StudentAssignedOfferings.ServiceOffering.Provider").
                                                                       SingleOrDefault(s => s.Id == viewModel.Id);
            if (updatedServiceRequest == null)
            {
                throw new EntityNotFoundException("Cannot find specified service request.");
            }
            IPermission permission = PermissionFactory.Current.Create("EditRequest", updatedServiceRequest);
            permission.GrantAccess(user);
            var currentServiceRequestFulfillment = updatedServiceRequest.FulfillmentDetails.OrderByDescending(f => f.CreateTime).FirstOrDefault();
            int currentStatusId = currentServiceRequestFulfillment.FulfillmentStatusId;
            int? currentOfferingId = currentServiceRequestFulfillment.FulfilledById;
            viewModel.CopyTo(updatedServiceRequest);
            if (currentStatusId != viewModel.SelectedStatusId)
            {
                CreateFulfillmentDetail(updatedServiceRequest, user, viewModel);
            }
            else if(currentOfferingId != viewModel.SelectedAssignedOfferingId)
            {
                UpdateCurrentFulfillmentDetail(updatedServiceRequest, viewModel);
            }
            updatedServiceRequest.LastModifyingUser = user.Identity.User;
            updatedServiceRequest.LastModifyTime = DateTime.Now;
            ServiceRequestRepository.Update(updatedServiceRequest);
            RepositoryContainer.Save();
        }

        private void CreateFulfillmentDetail(ServiceRequest model, EducationSecurityPrincipal user, ServiceRequestModel viewModel)
        {
            int selectedStatusId = viewModel.SelectedStatusId != 0 ? viewModel.SelectedStatusId : 1;
            var newDetail = new ServiceRequestFulfillment
            {
                FulfilledById = viewModel.SelectedAssignedOfferingId,
                CreatingUser = user.Identity.User,
                ServiceRequest = model,
                FulfillmentStatusId = selectedStatusId,
                Notes = viewModel.FulfillmentNotes
            };
            if (model.FulfillmentDetails == null)
            {
                model.FulfillmentDetails = new List<ServiceRequestFulfillment> { newDetail };
            }
            else
            {
                model.FulfillmentDetails.Add(newDetail);
            }
        }

        private void UpdateCurrentFulfillmentDetail(ServiceRequest model, ServiceRequestModel viewModel)
        {
            var currentFulfillment = ServiceRequestFulfillmentRepository.Items.OrderByDescending(s => s.CreateTime).FirstOrDefault(s => s.ServiceRequestId == model.Id);
            if (currentFulfillment != null)
            {
                currentFulfillment.FulfilledById = viewModel.SelectedAssignedOfferingId;
                ServiceRequestFulfillmentRepository.Update(currentFulfillment);
            }
        }

        public void Create(EducationSecurityPrincipal user, ServiceRequestModel viewModel)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            IPermission permission = PermissionFactory.Current.Create("CreateServiceRequest", StudentRepository.Items.Include(s => s.School.UserRoles).Where(s => viewModel.StudentIds.Contains(s.Id)));
            permission.GrantAccess(user);
            List<int> studentIds = viewModel.StudentIds.ToList();
            foreach (int studentId in studentIds)
            {
                ServiceRequest request = new ServiceRequest();
                viewModel.CopyTo(request);
                request.StudentId = studentId;
                request.CreatingUser = user.Identity.User;
                request.CreatingUserId = user.Identity.User.Id;
                CreateFulfillmentDetail(request, user, viewModel);
                ServiceRequestRepository.Add(request);
            }
            RepositoryContainer.Save();
        }

        public void Delete(EducationSecurityPrincipal user, int requestId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var serviceRequestToDelete = ServiceRequestRepository.Items.
                                                                  Include(s => s.ServiceType).
                                                                  Include(s => s.FulfillmentDetails).
                                                                  Include(s => s.Student.ApprovedProviders).
                                                                  Include(s => s.Student.School.UserRoles).
                                                                  SingleOrDefault(s => s.Id == requestId);
            if (serviceRequestToDelete == null)
            {
                throw new EntityNotFoundException("Service Request not found");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteRequest", serviceRequestToDelete);
            permission.GrantAccess(user);
            ServiceRequestRepository.Remove(serviceRequestToDelete);
            RepositoryContainer.Save();
        }
    }
}