using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strive.Domain
{
    public class Duration
    {
        public int Occurances { get; set; }
        public int Frequency { get; set; }
        public enum FrequencyOptions { Daily = 1, Weekly, Monthly, Yearly };
    }
}
