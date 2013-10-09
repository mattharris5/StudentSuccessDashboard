using SSD.Domain;
using System.Collections.Generic;

namespace SSD.IO
{
    public class StudentProfileExportFieldDescriptor
    {
        public StudentProfileExportFieldDescriptor()
        {
            SelectedCustomFields = new List<CustomField>();
            SelectedServiceTypes = new List<ServiceType>();
        }

        public bool BirthDateIncluded { get; set; }
        public bool ParentNameIncluded { get; set; }
        public IEnumerable<CustomField> SelectedCustomFields { get; set; }
        public IEnumerable<ServiceType> SelectedServiceTypes { get; set; }
    }
}
