using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace SSD.Domain
{
    public class CustomFieldValue
    {
        public int Id { get; internal set; }

        [Required]
        public int CustomFieldId { get; set; }
        public CustomField CustomField { get; set; }

        [Required]
        public int CustomDataOriginId { get; set; }
        public CustomDataOrigin CustomDataOrigin { get; set; }

        public string Value { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        public List<PrivateHealthDataViewEvent> PrivateHealthDataViewEvents { get; set; }
    }
}
