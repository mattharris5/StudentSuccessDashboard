using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class RemoveApprovedProvidersBySchoolModel
    {
        public MultiSelectList Schools { get; set; }
        public IEnumerable<int> SelectedSchools { get; set; }
    }
}
