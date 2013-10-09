using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ProviderModel : IStateCopier<Provider>
    {
        public int Id { get; set; }
        [Required(ErrorMessage="Provider Name is required")]
        public string Name { get; set; }
        public Address Address { get; set; }
        [Url]
        public string Website { get; set; }
        public Contact Contact { get; set; }

        public MultiSelectList Programs { get; set; }
        public IEnumerable<int> SelectedPrograms { get; set; }

        public void CopyTo(Provider model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.Name = Name;
            model.Address = Address;
            model.Contact = Contact;
            model.Website = Website;
        }

        public void CopyFrom(Provider model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            Id = model.Id;
            Name = model.Name;
            Address = model.Address;
            Contact = model.Contact;
            Website = model.Website;
        }
    }
}
