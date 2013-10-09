using SSD.Domain;
using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class ServiceOfferingListOptionsModel
    {
        public IEnumerable<ServiceOffering> Favorites { get; set; }
        public IEnumerable<string> CategoryFilterList { get; set; }
        public IEnumerable<string> TypeFilterList { get; set; }
    }
}
