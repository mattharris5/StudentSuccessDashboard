using System.Collections.Generic;

namespace SSD.ViewModels.DataTables
{
    public class DataTableResultModel
    {
        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public IEnumerable<object> aaData { get; set; }
    }
}
