using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class CustomFieldSelectorModel
    {
        public MultiSelectList CustomFields { get; set; }
        public IEnumerable<int> SelectedCustomFieldIds { get; set; }
    }
}
