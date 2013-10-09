using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class SchoolSelectorModel
    {
        public MultiSelectList Schools { get; set; }
        public IEnumerable<int> SelectedSchoolIds { get; set; }
    }
}
