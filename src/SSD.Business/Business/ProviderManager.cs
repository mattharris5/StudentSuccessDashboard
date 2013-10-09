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
    public class ProviderManager : IProviderManager
    {
        public ProviderManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder)
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
            ProviderRepository = repositories.Obtain<IProviderRepository>();
            SchoolRepository = repositories.Obtain<ISchoolRepository>();
            ProgramRepository = repositories.Obtain<IProgramRepository>();
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            DataTableBinder = dataTableBinder;
        }

        private IRepositoryContainer RepositoryContainer { get; set; }
        private IProviderRepository ProviderRepository { get; set; }
        private ISchoolRepository SchoolRepository { get; set; }
        private IProgramRepository ProgramRepository { get; set; }
        private IServiceOfferingRepository ServiceOfferingRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }

        public void PopulateViewModel(ProviderModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            viewModel.Programs = new MultiSelectList(ProgramRepository.Items.Where(p => p.IsActive).OrderBy(p => p.Name), "Id", "Name", viewModel.SelectedPrograms);
        }

        public ProviderModel GenerateEditViewModel(EducationSecurityPrincipal user, int providerId)
        {
            var provider = ProviderRepository.Items.Include("ServiceOfferings.Program").SingleOrDefault(p => p.Id == providerId);
            if (provider == null || !provider.IsActive)
            {
                throw new EntityNotFoundException("Specified Provider does not exist.");
            }
            IPermission permission = PermissionFactory.Current.Create("EditProvider", providerId);
            permission.GrantAccess(user);
            ProviderModel viewModel = new ProviderModel();
            viewModel.CopyFrom(provider);
            viewModel.SelectedPrograms = provider.ServiceOfferings.Select(s => s.Program).Select(p => p.Id);
            PopulateViewModel(viewModel);
            return viewModel;
        }

        public void Edit(EducationSecurityPrincipal user, ProviderModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            Provider updatedProvider = ProviderRepository.Items
                                                         .Include("ServiceOfferings.Program.Schools")
                                                         .SingleOrDefault(p => p.Id == viewModel.Id);
            if (updatedProvider == null || !updatedProvider.IsActive)
            {
                throw new EntityNotFoundException("Specified Provider does not exist.");
            }
            IPermission permission = PermissionFactory.Current.Create("EditProvider", viewModel.Id);
            permission.GrantAccess(user);
            Validate(viewModel);
            viewModel.CopyTo(updatedProvider);
            ProviderRepository.Update(updatedProvider);
            UpdateProviderPrograms(viewModel.SelectedPrograms, updatedProvider);
            RepositoryContainer.Save();
        }

        public void Create(EducationSecurityPrincipal user, ProviderModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            var item = ProviderRepository.Items.Include("ServiceOfferings.Program.Schools").SingleOrDefault(p => p.Name == viewModel.Name && !p.IsActive);
            if (item == null)
            {
                item = new Provider();
                ProviderRepository.Add(item);
            }
            viewModel.Id = item.Id;
            item.IsActive = true;
            viewModel.CopyTo(item);
            UpdateProviderPrograms(viewModel.SelectedPrograms, item);
            RepositoryContainer.Save();
        }

        public ProviderModel GenerateDeleteViewModel(int id)
        {
            var provider = ProviderRepository.Items.SingleOrDefault(p => p.Id == id);
            if (provider == null)
            {
                throw new EntityNotFoundException("Specified Provider does not exist");
            }
            ProviderModel model = new ProviderModel();
            model.CopyFrom(provider);
            return model;
        }

        public void Delete(int id)
        {
            var providerToDelete = ProviderRepository.Items.
                                                      Include("ServiceOfferings.Program.Schools").
                                                      Include(p => p.ApprovingStudents).
                                                      Include(p => p.UserRoles).
                                                      SingleOrDefault(p => p.Id == id);
            if (providerToDelete == null)
            {
                throw new EntityNotFoundException("Specified Provider does not exist");
            }
            if (StudentAssignedOfferingRepository.Items.
                Where(s => s.ServiceOffering.ProviderId == id && s.IsActive).Any())
            {
                throw new ValidationException(new ValidationResult("Provider associated to active Student Assigned Offerings", new string[] { "Id" }), null, id);
            }
            providerToDelete.IsActive = false;
            UpdateProviderPrograms(Enumerable.Empty<int>(), providerToDelete);
            foreach (var toDeactivate in providerToDelete.ServiceOfferings)
            {
                toDeactivate.IsActive = false;
            }
            foreach (var toRemove in providerToDelete.ApprovingStudents.ToArray())
            {
                providerToDelete.ApprovingStudents.Remove(toRemove);
                toRemove.ApprovedProviders.Remove(providerToDelete);
            }
            foreach (var toRemove in providerToDelete.UserRoles.ToArray())
            {
                providerToDelete.UserRoles.Remove(toRemove);
                toRemove.Providers.Remove(providerToDelete);
            }
            RepositoryContainer.Save();
        }

        public void Validate(ProviderModel viewModel)
        {
            if (ProviderRepository.Items.Any(p => p.IsActive && p.Name == viewModel.Name && p.Id != viewModel.Id))
            {
                throw new ValidationException(new ValidationResult("Name already exists", new string[] { "Name" }), null, viewModel.Name);
            }
        }

        public IEnumerable<string> SearchProviderNames(string term)
        {
            return ProviderRepository.Items.Where(p => p.IsActive).CompletionListFor(s => new CompletionProjection { Value = s.Name }, term);
        }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Provider> dataTable)
        {
            IQueryable<Provider> items = ProviderRepository.Items.Where(p => p.IsActive);
            return DataTableBinder.Bind<Provider>(items, dataTable, requestModel);
        }

        private void UpdateProviderPrograms(IEnumerable<int> selectedProgramIds, Provider provider)
        {
            List<Program> selectedPrograms = selectedProgramIds == null || !selectedProgramIds.Any() ? new List<Program>() : ProgramRepository.Items.Include(p => p.ServiceOfferings).Where(p => selectedProgramIds.Contains(p.Id)).ToList();
            IEnumerable<ServiceOffering> serviceOfferingSet = ServiceOfferingRepository.Items.Include(s => s.Program.ServiceOfferings).Where(s => s.Program != null && s.ProviderId == provider.Id);
            foreach (Program program in selectedPrograms)
            {
                CreateServiceOfferings(program, provider);
            }
            RemoveServiceOfferings(selectedPrograms, serviceOfferingSet);
            DeactivatePrograms(provider.ServiceOfferings.Select(s => s.Program).Distinct().ToList());
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

        private void CreateServiceOfferings(Program program, Provider provider)
        {
            foreach (int serviceTypeId in program.ServiceOfferings.Select(s => s.ServiceTypeId).Distinct().ToArray())
            {
                if (!provider.ServiceOfferings.Any(o => o.Program == program && o.ServiceTypeId == serviceTypeId))
                {
                    ServiceOffering newOffering = new ServiceOffering
                    {
                        IsActive = true,
                        Provider = provider,
                        Program = program,
                        ProgramId = program.Id,
                        ServiceTypeId = serviceTypeId
                    };
                    provider.ServiceOfferings.Add(newOffering);
                    ServiceOfferingRepository.Add(newOffering);
                }
                else if (provider.ServiceOfferings.Any(o => o.Program == program && o.ServiceTypeId == serviceTypeId && !o.IsActive))
                {
                    provider.ServiceOfferings.Single(o => o.Program == program && o.ServiceTypeId == serviceTypeId && !o.IsActive).IsActive = true;
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
