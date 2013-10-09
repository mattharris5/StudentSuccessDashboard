using System.Collections.Generic;
using System.Web.Mvc;

namespace SSD.ViewModels
{
    public class GradeSelectorModel
    {
        public MultiSelectList Grades { get; set; }
        public IEnumerable<string> SelectedGrades { get; set; }
    }
}
