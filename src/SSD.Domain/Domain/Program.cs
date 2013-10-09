using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SSD.DataAnnotations;

namespace SSD.Domain
{
    public class Program
    {
        public Program()
        {
            Schools = new List<School>();
            ServiceOfferings = new List<ServiceOffering>();
            ContactInfo = new Contact();
        }

        public int Id { get; internal set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public ICollection<School> Schools { get; set; }

        [RequiredElements]
        public ICollection<ServiceOffering> ServiceOfferings { get; set; }

        public string Purpose { get; set; }

        public Contact ContactInfo { get; set; }
    }
}
