using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strive.Domain
{
    public class Day
    {
        public string Name { get; set; }
        public bool MorningAvailability { get; set; }
        public bool AfternoonAvailability { get; set; }
        public bool EveningAvailability { get; set; }
    }
}
