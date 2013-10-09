using System;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class Address
    {
        [StringLength(50)]
        public string City { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Street { get; set; }

        [StringLength(10)]
        public string Zip { get; set; }
    }
}
