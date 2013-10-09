using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class UserAssociationsModel
    {
        public string UserName { get; set; }

        public IList<string> SchoolNames { get; set; }

        public IList<string> ProviderNames { get; set; }
    }
}
