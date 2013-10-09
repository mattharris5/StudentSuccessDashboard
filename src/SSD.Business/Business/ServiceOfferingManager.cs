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

namespace SSD.Business
{
    public class ServiceOfferingManager : IServiceOfferingManager
    {
        private IRepositoryContainer RepositoryContainer { get; set; }
        private IProviderRepository ProviderRepository { get; set; }
        private IServiceTypeRepository ServiceTypeRepository { get; set; }
        private IStudentAssignedOfferingRepository StudentAssignedOfferingRepository { get; set; }
        private IServiceOfferingRepository ServiceOfferingRepository { get; set; }
        private IServiceTypeCategoryRepository CategoryRepository { get; set; }
        private IDataTableBinder DataTableBinder { get; set; }

        public ServiceOfferingManager(IRepositoryContainer repositories, IDataTableBinder dataTableBinder)
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
            ServiceTypeRepository = repositories.Obtain<IServiceTypeRepository>();
            StudentAssignedOfferingRepository = repositories.Obtain<IStudentAssignedOfferingRepository>();
            ServiceOfferingRepository = repositories.Obtain<IServiceOfferingRepository>();
            CategoryRepository = repositories.Obtain<IServiceTypeCategoryRepository>();
            DataTableBinder = dataTableBinder;
        }

        public void SetFavoriteState(EducationSecurityPrincipal user, int offeringId, bool isFavorite)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            User userEntity = user.Identity.User;
            ServiceOffering serviceOffering = ServiceOfferingRepository.Items.Include(u => u.UsersLinkingAsFavorite).SingleOrDefault(s => s.Id == offeringId);
            if (serviceOffering == null || !serviceOffering.IsActive)
            {
                throw new EntityNotFoundException("Service Offering with the specified ID was not found.");
            }
            IPermission permission = PermissionFactory.Current.Create("SetFavoriteServiceOffering", serviceOffering.ProviderId);
            permission.GrantAccess(user);
            if (isFavorite)
            {
                ServiceOfferingRepository.AddLink(serviceOffering, userEntity);
            }
            else
            {
                ServiceOfferingRepository.DeleteLink(serviceOffering, userEntity);
            }
            RepositoryContainer.Save();
        }

        public ServiceOfferingListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user)
        {
            return new ServiceOfferingListOptionsModel
            {
                Favorites = LoadFavorites(user),
                TypeFilterList = ServiceTypeRepository.Items.Where(s => s.IsActive).Select(t => t.Name).ToList(),
                CategoryFilterList = CategoryRepository.Items.Select(c => c.Name).ToList()
            };
        }

        public DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceOffering> dataTable)
        {
            IQueryable<ServiceOffering> items = ServiceOfferingRepository.Items.Where(s => s.IsActive);
            return DataTableBinder.Bind<ServiceOffering>(items, dataTable, requestModel);
        }

        public IEnumerable<ServiceOffering> LoadFavorites(EducationSecurityPrincipal user)
        {
            return LookupHelper.LoadFavorites(ServiceOfferingRepository, user);
        }

        public void CheckStudentAssignedOfferings(int serviceOfferingId)
        {
            if (!ServiceOfferingRepository.Items.Any(s => s.Id == serviceOfferingId))
            {
                throw new EntityNotFoundException("Service Offering not found for Id");
            }
            if (StudentAssignedOfferingRepository.Items.
                Any(s => s.IsActive && s.ServiceOffering.Id == serviceOfferingId))
            {
                throw new ValidationException("There are Student Assigned Offerings associated with this Service Offering");
            }
        }
    }
}
