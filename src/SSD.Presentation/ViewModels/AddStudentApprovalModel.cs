using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class AddStudentApprovalModel
    {
        public int StudentId { get; set; }
        public MultiSelectList Providers { get; set; }
        public IEnumerable<int> ProvidersToAdd { get; set; }
    }
}
