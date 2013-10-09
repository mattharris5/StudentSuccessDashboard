using SSD.Domain;
using SSD.Repository;
using SSD.Security;
using SSD.Security.Permissions;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace SSD.Business
{
    public class ServiceTypeManager : IServiceTypeManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IServiceOfferingRepository ServiceOfferingRepository { get; set; }
        private IServiceTypeCategoryRepository CategoryRepository { get; set; }
        private IProgramRepository ProgramRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }

        public ServiceTypeManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder)
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
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            CategoryRepository = repositories.Obtain<IServiceTypeCategoryRepository>();
            ProgramRepository = repositories.Obtain<IProgramRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            DataTableBinder = dataTableBinder;
        }

        public void SetPrivacy(EducationSecurityPrincipal user, int typeId, bool isPrivate)
        {
            IPermission permission = PermissionFactory.Current.Create("SetServiceTypePrivacy");
            permission.GrantAccess(user);
            ServiceType serviceType = ServiceTypeRepository.Items.SingleOrDefault(s => s.Id == typeId);
            if (serviceType == null)
            {
                throw new EntityNotFoundException("Service Type with the specified ID was not found.");
            }
            serviceType.IsPrivate = isPrivate;
            ServiceTypeRepository.Update(serviceType);
            RepositoryContainer.Save();
        }

        public void PopulateViewModel(EducationSecurityPrincipal user, ServiceTypeModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            viewModel.IsAdministrator = user.IsInRole(SecurityRoles.DataAdmin);
            viewModel.Categories = new MultiSelectList(CategoryRepository.Items, "Id", "Name", viewModel.SelectedCategories);
            viewModel.Programs = new MultiSelectList(ProgramRepository.Items.Where(p => p.IsActive), "Id", "Name", viewModel.SelectedPrograms);
        }

        public ServiceTypeListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            return new ServiceTypeListOptionsModel
            {
                AllowModifying = user.IsInRole(SecurityRoles.DataAdmin),
                CategoryFilterList = CategoryRepository.Items.Select(s => s.Name).ToList()
            };
        }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceType> dataTable)
        {
            IQueryable<ServiceType> items = ServiceTypeRepository.Items.Where(s => s.IsActive);
            return DataTableBinder.Bind<ServiceType>(items, dataTable, requestModel);
        }

        public ServiceTypeModel GenerateCreateViewModel(EducationSecurityPrincipal user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            ServiceTypeModel model = new ServiceTypeModel();
            PopulateViewModel(user, model);
            return model;
        }

        public ServiceTypeModel GenerateEditViewModel(EducationSecurityPrincipal user, int typeId)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            var serviceType = ServiceTypeRepository.Items.Include(s => s.Categories).Include("ServiceOfferings.Program").SingleOrDefault(s => s.Id == typeId && s.IsActive);
            if (serviceType == null)
            {
                throw new EntityNotFoundException("Specified service type does not exist");
            }
            ServiceTypeModel viewModel = new ServiceTypeModel();
            viewModel.CopyFrom(serviceType);
            viewModel.Categories = new MultiSelectList(CategoryRepository.Items, "Id", "Name", serviceType.Categories.Select(c => c.Id));
            viewModel.Programs = new MultiSelectList(ProgramRepository.Items.Where(p => p.IsActive), "Id", "Name", serviceType.ServiceOfferings.Where(so => so.IsActive && so.Program.IsActive).Select(s => s.ProgramId).Distinct());
            viewModel.IsAdministrator = user.IsInRole(SecurityRoles.DataAdmin);
            return viewModel;
        }

        public ServiceTypeModel GenerateDeleteViewModel(int typeId)
        {
            var model = ServiceTypeRepository.Items.SingleOrDefault(s => s.Id == typeId);
            if (model == null)
            {
                throw new EntityNotFoundException("Specified service type does not exist");
            }
            ServiceTypeModel viewModel = new ServiceTypeModel();
            viewModel.CopyFrom(model);
            return viewModel;
        }

        public IEnumerable<string> SearchNames(string term)
        {
            return ServiceTypeRepository.Items.Where(s => s.IsActive).CompletionListFor(s => new CompletionProjection { Value = s.Name }, term);
        }

        public void Create(ServiceTypeModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            ServiceType item = new ServiceType { IsActive = true };
            viewModel.CopyTo(item);
            if (ServiceTypeRepository.Items.Any(s => s.Name == item.Name && !s.IsActive))
            {
                item = ServiceTypeRepository.Items.Include("ServiceOfferings.Program.Schools").Single(s => s.Name == item.Name);
                item.IsActive = true;
            }
            else
            {
                ServiceTypeRepository.Add(item);
            }
            UpdateTypeCategories(viewModel.SelectedCategories, item);
            UpdateServiceTypePrograms(viewModel.SelectedPrograms, item);
            RepositoryContainer.Save();
        }

        public void Edit(ServiceTypeModel viewModel)
        {
            var updatedServiceType = ServiceTypeRepository.Items.Include("ServiceOfferings.Program.Schools").Include(s => s.Categories).Include("ServiceOfferings.Program").SingleOrDefault(s => s.Id == viewModel.Id);
            if (updatedServiceType == null)
            {
                throw new EntityNotFoundException("Service Type not found.");
            }
            viewModel.CopyTo(updatedServiceType);
            ServiceTypeRepository.Update(updatedServiceType);
            UpdateTypeCategories(viewModel.SelectedCategories, updatedServiceType);
            UpdateServiceTypePrograms(viewModel.SelectedPrograms, updatedServiceType);
            RepositoryContainer.Save();
        }

        public void Delete(int typeId)
        {
            var serviceTypeToDelete = ServiceTypeRepository.Items.Include("ServiceOfferings.Program.Schools").SingleOrDefault(s => s.Id == typeId);
            if (serviceTypeToDelete == null)
            {
                throw new EntityNotFoundException("Service Type not found.");
            }
            if (StudentAssignedOfferingRepository.Items.
                Any(s => s.ServiceOffering.ServiceTypeId == typeId && s.IsActive))
            {
                throw new ValidationException(new ValidationResult("Service Type associated to active Student Assigned Offerings", new string[] { "Id" }), null, typeId);
            }
            serviceTypeToDelete.IsActive = false;
            UpdateServiceTypePrograms(Enumerable.Empty<int>(), serviceTypeToDelete);
            foreach(var toDeactivate in serviceTypeToDelete.ServiceOfferings)
            {
                toDeactivate.IsActive = false;
            }
            RepositoryContainer.Save();
        }

        public void ValidateForDuplicate(ServiceTypeModel viewModel)
        {
            if (ServiceTypeRepository.Items.Any(p => p.Name == viewModel.Name && p.Id != viewModel.Id && p.IsActive))
            {
                throw new ValidationException(new ValidationResult("Name already exists.", new string[] { "Name" }), null, viewModel);
            }
        }

        public ServiceTypeSelectorModel GenerateSelectorViewModel()
        {
            ServiceTypeSelectorModel viewModel = new ServiceTypeSelectorModel();
            viewModel.ServiceTypes = new MultiSelectList(ServiceTypeRepository.Items.Where(s => s.IsActive), "Id", "Name");
            return viewModel;
        }

        private void UpdateTypeCategories(IEnumerable<int> SelectedCategories, ServiceType serviceType)
        {
            List<Category> selectedCategories = (SelectedCategories == null || !SelectedCategories.Any()) ?
                                                    new List<Category>() :
                                                    CategoryRepository.Items.Where(s => SelectedCategories.Contains(s.Id)).ToList();
            IEnumerable<Category> categorySet = selectedCategories.Union(serviceType.Categories.ToArray());
            foreach (Category category in categorySet)
            {
                if (!selectedCategories.Contains(category))
                {
                    ServiceTypeRepository.DeleteLink(serviceType, category);
                }
                else if (!serviceType.Categories.Contains(category))
                {
                    ServiceTypeRepository.AddLink(serviceType, category);
                }
            }
        }

        private void UpdateServiceTypePrograms(IEnumerable<int> selectedProgramIds, ServiceType serviceType)
        {
            List<Program> selectedPrograms = selectedProgramIds == null || !selectedProgramIds.Any() ? new List<Program>() : ProgramRepository.Items.Include(p => p.ServiceOfferings).Where(p => selectedProgramIds.Contains(p.Id)).ToList();
            IEnumerable<ServiceOffering> serviceOfferingSet = ServiceOfferingRepository.Items.Include(s => s.Program.ServiceOfferings).Where(s => s.ServiceTypeId == serviceType.Id);
            foreach (Program program in selectedPrograms)
            {
                CreateServiceOfferings(program, serviceType);
            }
            RemoveServiceOfferings(selectedPrograms, serviceOfferingSet);
            DeactivatePrograms(serviceType.ServiceOfferings.Select(s => s.Program).Distinct().ToList());
        }

        private void RemoveServiceOfferings(IEnumerable<Program> selectedPrograms, IEnumerable<ServiceOffering> serviceOfferingSet)
        {
            var serviceOfferingsNotForSelectedProgramSet = serviceOfferingSet.Where(s => !selectedPrograms.Contains(s.Program));
            var ids = serviceOfferingsNotForSelectedProgramSet.Select(so => so.Id);
            if (StudentAssignedOfferingRepository.Items.
                Any(s => ids.Contains(s.ServiceOfferingId) && s.IsActive))
            {
                throw new ValidationException(new ValidationResult("One or more of the Service Offerings that will be deactivated has active Student Assigned Offerings", new string[] { "Id" }), null, null);
            }
            foreach (var toRemove in serviceOfferingsNotForSelectedProgramSet)
            {
                toRemove.IsActive = false;
            }
        }

        private void CreateServiceOfferings(Program program, ServiceType serviceType)
        {
            foreach (int providerId in program.ServiceOfferings.Select(s => s.ProviderId).Distinct().ToArray())
            {
                if (!serviceType.ServiceOfferings.Any(o => o.Program == program && o.ProviderId == providerId))
                {
                    ServiceOffering newOffering = new ServiceOffering
                    {
                        IsActive = true,
                        ServiceType = serviceType,
                        Program = program,
                        ProgramId = program.Id,
                        ProviderId = providerId
                    };
                    serviceType.ServiceOfferings.Add(newOffering);
                    ServiceOfferingRepository.Add(newOffering);
                }
                else if (serviceType.ServiceOfferings.Any(o => o.Program == program && o.ProviderId == providerId && !o.IsActive))
                {
                    serviceType.ServiceOfferings.Single(o => o.Program == program && o.ProviderId == providerId && !o.IsActive).IsActive = true;
                }
            }
        }

        private void DeactivatePrograms(List<Program> programs) 
        {
            var programsToDeactivate = programs.
                Where(p => p != null && 
                    !p.ServiceOfferings.Any(s => s.IsActive)).ToList();
            foreach (var program in programsToDeactivate)
            {
                program.IsActive = false;
                var schoolsToDelete = program.Schools.ToArray();
                foreach (var school in schoolsToDelete)
                {
                    program.Schools.Remove(school);
                }
            }
        }
    }
}
