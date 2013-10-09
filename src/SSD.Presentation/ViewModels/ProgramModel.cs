using SSD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using SSD.DataAnnotations;

namespace SSD.ViewModels
{
    public class ProgramModel : IStateCopier<Program>
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255)]
        public string Name { get; set; }
        public string Purpose { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        [EmailAddress]
        public string ContactEmail { get; set; }

        public MultiSelectList Providers { get; set; }
        [RequiredElements]
        public IEnumerable<int> SelectedProviders { get; set; }

        public MultiSelectList ServiceTypes { get; set; }
        [RequiredElements]
        public IEnumerable<int> SelectedServiceTypes { get; set; }

        public MultiSelectList Schools { get; set; }

        public IEnumerable<int> SelectedSchools { get; set; }

        public void CopyTo(Program model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            model.ContactInfo.Email = ContactEmail;
            model.ContactInfo.Name = ContactName;
            model.ContactInfo.Phone = ContactPhone;
            model.EndDate = EndDate;
            model.StartDate = StartDate;
            model.Name = Name;
            model.Purpose = Purpose;
        }

        public void CopyFrom(Program model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }
            ContactEmail = model.ContactInfo.Email;
            ContactName = model.ContactInfo.Name;
            ContactPhone = model.ContactInfo.Phone;
            EndDate = model.EndDate;
            StartDate = model.StartDate;
            Name = model.Name;
            Purpose = model.Purpose;
        }
    }
}
