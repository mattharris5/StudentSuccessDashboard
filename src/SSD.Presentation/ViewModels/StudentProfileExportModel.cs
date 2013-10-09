using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class StudentProfileExportModel
    {
        public StudentProfileExportModel()
        {
            SelectedCustomFieldIds = new List<int>();
            SelectedServiceTypeIds = new List<int>();
            SelectedSchoolIds = new List<int>();
            SelectedGrades = new List<int>();
        }

        public bool BirthDateIncluded { get; set; }
        public bool ParentNameIncluded { get; set; }

        public IEnumerable<int> SelectedCustomFieldIds { get; set; }

        public IEnumerable<int> SelectedServiceTypeIds { get; set; }

        public IEnumerable<int> SelectedSchoolIds { get; set; }

        public IEnumerable<int> SelectedGrades { get; set; }
    }
}
