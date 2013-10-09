using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class StudentApprovalListOptionsModel
    {
        public IEnumerable<string> SchoolFilterList { get; set; }
        public IEnumerable<string> ProviderFilterList { get; set; }
        public int TotalStudentsWithApproval { get; set; }
    }
}
