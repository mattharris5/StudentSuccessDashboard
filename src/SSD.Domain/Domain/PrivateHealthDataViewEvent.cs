using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SSD.Domain
{
    public class PrivateHealthDataViewEvent : IAuditCreate
    {
        public PrivateHealthDataViewEvent()
        {
            CreateTime = DateTime.Now;
        }

        public int Id { get; internal set; }

        public DateTime CreateTime { get; internal set; }

        [Required]
        public int CreatingUserId { get; set; }
        public User CreatingUser { get; set; }

        public List<CustomFieldValue> PhiValuesViewed { get; set; }
    }
}
