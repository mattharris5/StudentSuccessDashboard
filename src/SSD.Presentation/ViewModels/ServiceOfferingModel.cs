using SSD.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ServiceOfferingModel :  IStateCopier<ServiceOffering>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Service Type is required")]
        public int SelectedServiceTypeId { get; set; }
        public SelectList ServiceTypes { get; set; }

        [Required(ErrorMessage = "Provider is required")]
        public int SelectedProviderId { get; set; }
        public SelectList Providers { get; set; }

        public bool IsFavorite { get; set; }
        
        public void CopyTo(ServiceOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.ProviderId = SelectedProviderId;
            model.ServiceTypeId = SelectedServiceTypeId;
        }

        public void CopyFrom(ServiceOffering model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            SelectedProviderId = model.ProviderId;
            SelectedServiceTypeId = model.ServiceTypeId;
        }
    }
}
