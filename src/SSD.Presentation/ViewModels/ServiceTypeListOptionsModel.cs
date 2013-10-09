using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ServiceTypeListOptionsModel
    {
        public bool AllowModifying { get; set; }
        public IEnumerable<string> CategoryFilterList { get; set; }
    }
}
