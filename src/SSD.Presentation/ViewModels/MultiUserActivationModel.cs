using System.Collections.Generic;

namespace SSD.ViewModels
{
    public class MultiUserActivationModel
    {
        public string ActivationString { get; set; }
        public IEnumerable<int> UserIds { get; set; }
        public bool ActiveFlag { get; set; }

        public MultiUserActivationModel()
        {
            UserIds = new List<int>();
        }

        public void InitializeValues(IEnumerable<int> ids, bool activeStatus, string activationString) 
        {
            ActivationString = activationString;
            UserIds = ids;
            ActiveFlag = activeStatus;
        }
    }

}
