using System;

namespace SSD.ViewModels
{
    public class ServiceRequestsBySchoolModel
    {
        public string SchoolName { get; set; }
        public int Total { get; set; }
        public int Open { get; set; }
        public int Fulfilled { get; set; }
    }
}
