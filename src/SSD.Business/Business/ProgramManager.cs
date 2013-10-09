using SSD.Domain;
using SSD.Repository;
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
    public class ProgramManager : IProgramManager
    {
        public ProgramManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder)
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
            ProgramRepository = repositories.Obtain<IProgramRepository>();
            ProviderRepository = repositories.Obtain<IProviderRepository>();
            SchoolRepository = repositories.Obtain<ISchoolRepository>();
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            DataTableBinder = dataTableBinder;
        }

        private IRepositoryContainer RepositoryContainer { get; set; }
        private IProgramRepository ProgramRepository { get; set; }
        private IProviderRepository ProviderRepository { get; set; }
        private ISchoolRepository SchoolRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IServiceOfferingRepository ServiceOfferingRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<Program> dataTable)
        {
            IQueryable<Program> items = ProgramRepository.Items.Where(p => p.IsActive);
            return DataTableBinder.Bind<Program>(items, dataTable, requestModel);
        }

        public void PopulateViewModelLists(ProgramModel viewModel)
        {
            viewModel.Providers = new MultiSelectList(ProviderRepository.Items.Where(p => p.IsActive).OrderBy(p => p.Name), "Id", "Name", viewModel.SelectedProviders);
            viewModel.ServiceTypes = new MultiSelectList(ServiceTypeRepository.Items.Where(s => s.IsActive).OrderBy(s => s.Name), "Id", "Name", viewModel.SelectedServiceTypes);
            viewModel.Schools = new MultiSelectList(SchoolRepository.Items.OrderBy(s => s.Name), "Id", "Name", viewModel.SelectedSchools);
        }

        public ProgramModel GenerateCreateViewModel()
        {
            ProgramModel viewModel = new ProgramModel();
            return viewModel;
        }

        public void Create(ProgramModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            Program item = ProgramRepository.Items.SingleOrDefault(p => p.Name == viewModel.Name && !p.IsActive);
            if (item == null)
            {
               item = new Program();
               ProgramRepository.Add(item);
            }
            viewModel.Id = item.Id;
            item.IsActive = true;
            viewModel.CopyTo(item);
            UpdateSchools(viewModel, item);
            var mappings = GenerateServiceOfferingMappings(viewModel, item);
            foreach (var mapping in mappings)
            {
                if (!ServiceOfferingRepository.Items.Where(s => s.ProgramId == mapping.ProgramId && s.ProviderId == mapping.ProviderId && s.ServiceTypeId == mapping.ServiceTypeId).Any()) 
                {
                    item.ServiceOfferings.Add(mapping);
                    ServiceOfferingRepository.Add(mapping);
                }
            }
            RepositoryContainer.Save();
        }

        private List<ServiceOffering> GenerateServiceOfferingMappings(ProgramModel viewModel, Program item)
        {
            List<ServiceOffering> newMappings = new List<ServiceOffering>();
            if (viewModel.SelectedServiceTypes != null && viewModel.SelectedProviders != null)
            {
                foreach (var serviceTypeId in viewModel.SelectedServiceTypes)
                {
                    foreach (var providerId in viewModel.SelectedProviders)
                    {
                        ServiceOffering newOffering = ServiceOfferingRepository.Items.SingleOrDefault(s => s.ServiceTypeId == serviceTypeId && s.ProviderId == providerId && s.ProgramId == item.Id && !s.IsActive);
                        if (newOffering == null)
                        {
                            newOffering = new ServiceOffering { ProviderId = providerId, ServiceTypeId = serviceTypeId, Program = item, ProgramId = item.Id, IsActive = true };
                        }
                        else
                        {
                            newOffering.IsActive = true;
                        }
                        newMappings.Add(newOffering);
                    }
                }
            }
            return newMappings;
        }

        public ProgramModel GenerateEditViewModel(int id)
        {
            Program program = ProgramRepository.Items.Include(p => p.ServiceOfferings).Include(p => p.Schools).SingleOrDefault(p => p.Id == id);
            if (program == null || !program.IsActive)
            {
                throw new EntityNotFoundException("Could not find Program with specified Id.");
            }
            ProgramModel viewModel = new ProgramModel();
            viewModel.CopyFrom(program);
            viewModel.SelectedProviders = program.ServiceOfferings.Where(s => s.IsActive).Select(s => s.ProviderId);
            viewModel.SelectedServiceTypes = program.ServiceOfferings.Where(s => s.IsActive).Select(s => s.ServiceTypeId);
            viewModel.SelectedSchools = program.Schools.Select(s => s.Id);
            return viewModel;
        }

        public void Edit(ProgramModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException("viewModel");
            }
            Program toUpdate = ProgramRepository.Items.Include(s => s.ServiceOfferings).Include(s => s.Schools).SingleOrDefault(p => p.Id == viewModel.Id);
            if (toUpdate == null || !toUpdate.IsActive)
            {
                throw new EntityNotFoundException("Could not find Program with specified Id.");
            }
            var currentMappings = ServiceOfferingRepository.Items.
                Include(c => c.ServiceType).
                Include(c => c.Provider).
                Include("StudentAssignedOfferings").
                Where(s => s.ProgramId == viewModel.Id).ToList();
            var newMappings = GenerateServiceOfferingMappings(viewModel, toUpdate);
            UpdateSchools(viewModel, toUpdate);
            DeactivateServiceOfferings(currentMappings, newMappings);
            ActivateServiceOfferings(currentMappings, newMappings);
            viewModel.CopyTo(toUpdate);
            ProgramRepository.Update(toUpdate);
            RepositoryContainer.Save();
        }

        private void UpdateSchools(ProgramModel model, Program toUpdate)
        {
            var selectedSchools = (model.SelectedSchools == null || !model.SelectedSchools.Any()) ?
                new List<School>() :
                SchoolRepository.Items.Where(s => model.SelectedSchools.Contains(s.Id)).ToList();
            toUpdate.Schools = selectedSchools;
        }

        private void ActivateServiceOfferings(List<ServiceOffering> currentMappings, List<ServiceOffering> newMappings)
        {
            var offeringsToActivate = currentMappings.Where(c => newMappings.Any(n => n.ProviderId == c.ProviderId && n.ServiceTypeId == c.ServiceTypeId) && !c.IsActive);
            foreach (var offering in offeringsToActivate)
            {
                offering.IsActive = true;
            }
            var offeringsToAdd = newMappings.Where(n => !currentMappings.Any(c => n.ProviderId == c.ProviderId && n.ServiceTypeId == c.ServiceTypeId));
            foreach (var offering in offeringsToAdd)
            {
                offering.Program.ServiceOfferings.Add(offering);
                ServiceOfferingRepository.Add(offering);
            }
        }

        private void DeactivateServiceOfferings(List<ServiceOffering> currentMappings, List<ServiceOffering> newMappings)
        {
            var offeringsToDeactivate = currentMappings.Where(c => !newMappings.Any(n => n.ProviderId == c.ProviderId && n.ServiceTypeId == c.ServiceTypeId));
            foreach (var offering in offeringsToDeactivate)
            {
                if (offering.StudentAssignedOfferings.Any(s => s.IsActive))
                {
                    throw new ValidationException(string.Format("There are still active Student Assigned Offerings for Service Type {0} and Provider {1}.", offering.ServiceType.Name, offering.Provider.Name));
                }
                offering.IsActive = false;
                ServiceOfferingRepository.Update(offering);
            }
        }

        public ProgramModel GenerateDeleteViewModel(int id)
        {
            Program program = ProgramRepository.Items.SingleOrDefault(p => p.Id == id);
            if (program == null)
            {
                throw new EntityNotFoundException("Could not find Program with specified Id.");
            }
            ProgramModel viewModel = new ProgramModel();
            viewModel.CopyFrom(program);
            return viewModel;
        }

        public void Delete(int id)
        {
            Program toDelete = ProgramRepository.Items.Include(p => p.Schools).Include(p => p.ServiceOfferings).SingleOrDefault(p => p.Id == id);
            if (toDelete == null)
            {
                throw new EntityNotFoundException("Could not find Program with specified Id.");
            }
            if (StudentAssignedOfferingRepository.Items.Any(s => s.ServiceOffering.ProgramId == id && s.IsActive))
            {
                throw new ValidationException(new ValidationResult("Program associated to active Student Assigned Offerings", new string[] { "Id" }), null, id);
            }
            toDelete.IsActive = false;
            foreach (var toDeactivate in toDelete.ServiceOfferings)
            {
                toDeactivate.IsActive = false;
            }
            toDelete.IsActive = false;
            var schoolsToDelete = toDelete.Schools.ToArray();
            foreach (var school in schoolsToDelete)
            {
                toDelete.Schools.Remove(school);
            }
            RepositoryContainer.Save();
        }

        public IEnumerable<string> SearchProgramNames(string term)
        {
            return ProgramRepository.Items.Where(s => s.IsActive).CompletionListFor(s => new CompletionProjection { Value = s.Name }, term);
        }

        private IEnumerable<Provider> LoadSelectedProviders(ProgramModel viewModel)
        {
            if (viewModel.SelectedProviders == null)
            {
                return Enumerable.Empty<Provider>();
            }
            return ProviderRepository.Items.Where(p => viewModel.SelectedProviders.Contains(p.Id)).ToList();
        }

        private IEnumerable<ServiceType> LoadSelectedServiceTypes(ProgramModel viewModel)
        {
            if (viewModel.SelectedServiceTypes == null)
            {
                return Enumerable.Empty<ServiceType>();
            }
            return ServiceTypeRepository.Items.Where(t => viewModel.SelectedServiceTypes.Contains(t.Id)).ToList();
        }


        public void Validate(ProgramModel viewModel)
        {
            if (ProgramRepository.Items.Any(p => p.IsActive && p.Name == viewModel.Name && p.Id != viewModel.Id))
            {
                throw new ValidationException(new ValidationResult("Name already exists", new string[] { "Name" }), null, viewModel.Name);
            }
        }
    }
}
