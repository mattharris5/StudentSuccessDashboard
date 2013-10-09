using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ServiceTypeModel : IStateCopier<ServiceType>
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Service Type Name is required")]
        public string Name { get; set; }
        public string Description { get; set; }
        public MultiSelectList Categories { get; set; }
        public IEnumerable<int> SelectedCategories { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsAdministrator { get; set; }
        public MultiSelectList Programs { get; set; }
        public IEnumerable<int> SelectedPrograms { get; set; }

        public void CopyTo(ServiceType model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.Name = Name;
            model.Description = Description;
            model.IsPrivate = IsPrivate;
        }

        public void CopyFrom(ServiceType model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            Name = model.Name;
            Description = model.Description;
            IsPrivate = model.IsPrivate;
        }
    }
}
