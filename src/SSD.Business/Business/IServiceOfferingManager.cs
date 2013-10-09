using SSD.Domain;
using SSD.Security;
using SSD.ViewModels;
using SSD.ViewModels.DataTables;
using System.Collections.Generic;

namespace SSD.Business
{
    public interface IServiceOfferingManager
    {
        ServiceOfferingListOptionsModel GenerateListOptionsViewModel(EducationSecurityPrincipal user);
        DataTableResultModel GenerateDataTableResultViewModel(DataTableRequestModel requestModel, IClientDataTable<ServiceOffering> dataTable);
        void SetFavoriteState(EducationSecurityPrincipal user, int offeringId, bool isFavorite);
        IEnumerable<ServiceOffering> LoadFavorites(EducationSecurityPrincipal user);
        void CheckStudentAssignedOfferings(int serviceOfferingId);
    }
}
