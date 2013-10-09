using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    public class ServiceAttendanceManager : IServiceAttendanceManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IServiceAttendanceRepository ServiceAttendanceRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private ISubjectRepository SubjectRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }

        public ServiceAttendanceManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            if (dataTableBinder == null)
            {
                throw new ArgumentNullException("dataTableBinder");
            }
            RepositoryContainer = repositories;
            ServiceAttendanceRepository = repositories.Obtain<IServiceAttendanceRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            SubjectRepository = repositories.Obtain<ISubjectRepository>();
            DataTableBinder = dataTableBinder;
        }
        
        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceAttendance> dataTable)
        {
            if (requestModel == null)
            {
                throw new ArgumentNullException("requestModel");
            }
            IQueryable<ServiceAttendance> items = ServiceAttendanceRepository.Items;
            return DataTableBinder.Bind<ServiceAttendance>(items, dataTable, requestModel);
        }

        public ServiceAttendanceModel GenerateCreateViewModel(EducationSecurityPrincipal user, int id)
        {
            StudentAssignedOffering studentAssignedOffering = StudentAssignedOfferingRepository.Items.
                                                                                                Include(s => s.Student.School).
                                                                                                Include(s => s.ServiceOffering.Provider).
                                                                                                SingleOrDefault(s => s.Id == id);
            if (studentAssignedOffering == null)
            {
                throw new EntityNotFoundException("Could not find student assigned offering with given Id.");
            }
            IPermission permission = PermissionFactory.Current.Create("CreateServiceAttendance", studentAssignedOffering);
            permission.GrantAccess(user);
            var viewModel = new ServiceAttendanceModel() { StudentAssignedOfferingId = id, DateAttended = DateTime.Now, SelectedSubjectId = 1 };
            viewModel.Subjects = new SelectList(LookupHelper.LoadSubjectList(SubjectRepository), "Id", "Name", viewModel.SelectedSubjectId);
            return viewModel;
        }

        public ServiceAttendanceModel GenerateEditViewModel(EducationSecurityPrincipal user, int id)
        {
            var serviceAttendance = ServiceAttendanceRepository.Items.
                                    Include(s => s.CreatingUser).
                                    Include(s => s.LastModifyingUser).
                                    Include(s => s.StudentAssignedOffering).
                                    SingleOrDefault(s => s.Id == id);
            if (serviceAttendance == null)
            {
                throw new EntityNotFoundException("Specified service attendance does not exist");
            }
            IPermission permission = PermissionFactory.Current.Create("EditServiceAttendance", serviceAttendance.StudentAssignedOffering);
            permission.GrantAccess(user);
            var viewModel = new ServiceAttendanceModel();
            viewModel.CopyFrom(serviceAttendance);
            viewModel.Subjects = new SelectList(LookupHelper.LoadSubjectList(SubjectRepository), "Id", "Name", viewModel.SelectedSubjectId);
            return viewModel;
        }

        public ServiceAttendanceModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int id)
        {
            var serviceAttendance = ServiceAttendanceRepository.Items.Include(s => s.StudentAssignedOffering).SingleOrDefault(s => s.Id == id);
            if (serviceAttendance == null)
            {
                throw new EntityNotFoundException("Specified service attendance does not exist");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteServiceAttendance", serviceAttendance.StudentAssignedOffering);
            permission.GrantAccess(user);
            var viewModel = new ServiceAttendanceModel();
            viewModel.CopyFrom(serviceAttendance);
            return viewModel;
        }

        public void Create(ServiceAttendanceModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var studentAssignedOffering = StudentAssignedOfferingRepository.Items.Single(s => s.Id == viewModel.StudentAssignedOfferingId);
            IPermission permission = PermissionFactory.Current.Create("CreateServiceAttendance", studentAssignedOffering);
            permission.GrantAccess(user);
            ServiceAttendance serviceAttendance = new ServiceAttendance
            {
                CreatingUser = user.Identity.User
            };
            viewModel.CopyTo(serviceAttendance);
            ServiceAttendanceRepository.Add(serviceAttendance);
            RepositoryContainer.Save();
        }

        public void Edit(ServiceAttendanceModel viewModel, EducationSecurityPrincipal user)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var updatedServiceAttendance = ServiceAttendanceRepository.Items.Include(s => s.StudentAssignedOffering).SingleOrDefault(s => s.Id == viewModel.Id);
            if (updatedServiceAttendance == null)
            {
                throw new EntityNotFoundException();
            }
            IPermission permission = PermissionFactory.Current.Create("EditServiceAttendance", updatedServiceAttendance.StudentAssignedOffering);
            permission.GrantAccess(user);
            viewModel.CopyTo(updatedServiceAttendance);
            updatedServiceAttendance.LastModifyingUser = user.Identity.User;
            updatedServiceAttendance.LastModifyTime = DateTime.Now;
            ServiceAttendanceRepository.Update(updatedServiceAttendance);
            RepositoryContainer.Save();
        }

        public void Delete(int id, EducationSecurityPrincipal user)
        {
            var serviceAttendanceToDelete = ServiceAttendanceRepository.Items.Include(s => s.StudentAssignedOffering).SingleOrDefault(s => s.Id == id);
            if (serviceAttendanceToDelete == null)
            {
                throw new EntityNotFoundException("Specified Service Attendance does not exist");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteServiceAttendance", serviceAttendanceToDelete.StudentAssignedOffering);
            permission.GrantAccess(user);
            ServiceAttendanceRepository.Remove(serviceAttendanceToDelete);
            RepositoryContainer.Save();
        }
    }
}
