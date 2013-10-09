using SSD.Domain;
using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ScheduleServiceOfferingListOptionsModel
    {
        public string ReturnUrl { get; set; }
        public IEnumerable<int> SelectedStudents { get; set; }
        public IEnumerable<ServiceOffering> Favorites { get; set; }
        public IEnumerable<string> CategoryFilterList { get; set; }
        public IEnumerable<string> TypeFilterList { get; set; }
    }
}
