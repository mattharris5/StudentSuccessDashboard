using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class ServiceTypeSelectorModel
    {
        public MultiSelectList ServiceTypes { get; set; }
        public IEnumerable<int> SelectedServiceTypeIds { get; set; }
    }
}
