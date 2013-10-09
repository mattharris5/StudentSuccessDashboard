using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace SSD.Business
{
    public class ScheduledServiceManager : IScheduledServiceManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private IServiceOfferingRepository ServiceOfferingRepository { get; set; }
        private IStudentRepository StudentRepository { get; set; }
        private IServiceTypeCategoryRepository CategoryRepository { get; set; }

        public ScheduledServiceManager(IRepositoryContainer repositories)
        {
            if (repositories == null)
            {
                throw new ArgumentNullException("repositories");
            }
            RepositoryContainer = repositories;
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            StudentRepository = repositories.Obtain<IStudentRepository>();
            CategoryRepository = repositories.Obtain<IServiceTypeCategoryRepository>();
        }

        public ScheduleServiceOfferingListOptionsModel GenerateScheduleOfferingViewModel(EducationSecurityPrincipal user, IEnumerable<int> studentIds)
        {
            IEnumerable<Student> students = StudentRepository.Items.Include(s => s.School).Where(s => studentIds.Contains(s.Id));
            IPermission permission = PermissionFactory.Current.Create("ScheduleOffering", students);
            permission.GrantAccess(user);
            ScheduleServiceOfferingListOptionsModel viewModel = new ScheduleServiceOfferingListOptionsModel { SelectedStudents = studentIds };
            viewModel.Favorites = LookupHelper.LoadFavorites(ServiceOfferingRepository, user);
            viewModel.TypeFilterList = ServiceTypeRepository.Items.Where(s => s.IsActive).Select(t => t.Name).ToList();
            viewModel.CategoryFilterList = CategoryRepository.Items.Select(c => c.Name).ToList();
            return viewModel;
        }

        public ServiceOfferingScheduleModel GenerateCreateViewModel(int offeringId)
        {
            if (!(ServiceOfferingRepository.Items.Any(s => s.Id == offeringId && s.IsActive)))
            {
                throw new EntityNotFoundException("Specified Service Offering does not exist.");
            }
            return new ServiceOfferingScheduleModel { ServiceOfferingId = offeringId };
        }

        public StudentServiceOfferingScheduleModel GenerateEditViewModel(EducationSecurityPrincipal user, int scheduledOfferingId)
        {
            StudentAssignedOffering assignment = StudentAssignedOfferingRepository.Items.
                                                    Include(a => a.ServiceOffering.ServiceType).
                                                    Include(a => a.ServiceOffering.Provider).
                                                    Include(a => a.ServiceOffering.Program).
                                                    Include(a => a.CreatingUser).
                                                    Include(a => a.LastModifyingUser).
                                                    SingleOrDefault(a => a.Id == scheduledOfferingId);
            if (assignment == null || !assignment.IsActive)
            {
                throw new EntityNotFoundException("Scheduled Service Offering does not exist.");
            }
            IPermission permission = PermissionFactory.Current.Create("EditScheduledOffering", assignment);
            permission.GrantAccess(user);
            StudentServiceOfferingScheduleModel viewModel = new StudentServiceOfferingScheduleModel();
            viewModel.CopyFrom(assignment);
            return viewModel;
        }

        public DeleteServiceOfferingScheduleModel GenerateDeleteViewModel(EducationSecurityPrincipal user, int scheduledOfferingId)
        {
            StudentAssignedOffering assignment = StudentAssignedOfferingRepository.Items.
                                                    Include(s => s.ServiceOffering).
                                                    Include(s => s.ServiceOffering.Provider).
                                                    Include(s => s.ServiceOffering.ServiceType).
                                                    Include(s => s.ServiceOffering.Program).
                                                    SingleOrDefault(a => a.Id == scheduledOfferingId);
            if (assignment == null || !assignment.IsActive)
            {
                throw new EntityNotFoundException("Scheduled Service Offering does not exist.");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteScheduledOffering", assignment);
            permission.GrantAccess(user);
            return new DeleteServiceOfferingScheduleModel
            {
                Id = scheduledOfferingId,
                StudentId = assignment.StudentId,
                Name = assignment.ServiceOffering.Name
            };
        }

        public void Create(EducationSecurityPrincipal user, ServiceOfferingScheduleModel viewModel)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            if (!ServiceOfferingRepository.Items.Any(s => s.Id == viewModel.ServiceOfferingId && s.IsActive))
            {
                throw new EntityNotFoundException("Selected Service Offering was not found.");
            }
            ServiceOffering offering = ServiceOfferingRepository.Items.Single(s => s.Id == viewModel.ServiceOfferingId && s.IsActive);
            IEnumerable<Student> students = StudentRepository.Items.Include(s => s.School).Where(s => viewModel.SelectedStudents.Contains(s.Id));
            IPermission permission = PermissionFactory.Current.Create("ScheduleOffering", students, offering);
            permission.GrantAccess(user);
            User userEntity = user.Identity.User;
            List<int> studentIds = viewModel.SelectedStudents.ToList();
            foreach (int studentId in studentIds)
            {
                var studentAssignedOffering = new StudentAssignedOffering
                {
                    StudentId = studentId,
                    CreatingUserId = userEntity.Id,
                    IsActive = true
                };
                viewModel.CopyTo(studentAssignedOffering);
                StudentAssignedOfferingRepository.Add(studentAssignedOffering);
            }
            RepositoryContainer.Save();
        }

        public void Edit(EducationSecurityPrincipal user, StudentServiceOfferingScheduleModel viewModel)
        {
            var existing = StudentAssignedOfferingRepository.Items.
                           Include(s => s.ServiceOffering.ServiceType).
                           SingleOrDefault(a => a.Id == viewModel.Id);
            if (existing == null || !existing.IsActive)
            {
                throw new EntityNotFoundException("Assigned offering not found");
            }
            IPermission permission = PermissionFactory.Current.Create("EditScheduledOffering", existing);
            permission.GrantAccess(user);
            EditScheduledOffering(viewModel, existing, user);
            RepositoryContainer.Save();
        }

        public void Delete(EducationSecurityPrincipal user, int scheduledOfferingId)
        {
            StudentAssignedOffering assignment = StudentAssignedOfferingRepository.Items.
                                                 Include(s => s.ServiceOffering.ServiceType).
                                                 Include(s => s.Attendances).
                                                 Include(s => s.Student.School).
                                                 SingleOrDefault(s => s.Id == scheduledOfferingId);
            if (assignment == null || !assignment.IsActive)
            {
                throw new EntityNotFoundException("Requested assignment was not found");
            }
            IPermission permission = PermissionFactory.Current.Create("DeleteScheduledOffering", assignment);
            permission.GrantAccess(user);
            assignment.IsActive = false;
            StudentAssignedOfferingRepository.Update(assignment);
            RepositoryContainer.Save();
        }

        private void EditScheduledOffering(StudentServiceOfferingScheduleModel viewModel, StudentAssignedOffering existing, EducationSecurityPrincipal user)
        {
            existing.LastModifyingUser = user.Identity.User;
            existing.LastModifyTime = DateTime.Now;
            viewModel.CopyTo(existing);
            StudentAssignedOfferingRepository.Update(existing);
        }
    }
}
