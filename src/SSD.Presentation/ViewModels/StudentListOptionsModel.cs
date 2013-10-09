using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class StudentListOptionsModel
    {
        public bool IsProvider { get; set; }
        public IEnumerable<int> GradeFilterList { get; set; }
        public IEnumerable<string> SchoolFilterList { get; set; }
        public IEnumerable<string> PriorityFilterList { get; set; }
        public IEnumerable<string> RequestStatusFilterList { get; set; }
        public IEnumerable<string> ServiceTypeFilterList { get; set; }
        public IEnumerable<string> SubjectFilterList { get; set; }
    }
}
