using System;
using System.Collections.Generic;
using System.Data.Services.Common;
using System.Linq;
using System.Text;

namespace Strive.Domain
{
    [DataServiceKey("Id")]
    public class Availability
    {
        public Availability()
        {
            Monday = new Day { Name = "Monday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Tuesday = new Day { Name = "Tuesday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Wednesday = new Day { Name = "Wednesday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Thursday = new Day { Name = "Thursday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Friday = new Day { Name = "Friday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Saturday = new Day { Name = "Saturday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
            Sunday = new Day { Name = "Sunday", MorningAvailability = false, AfternoonAvailability = false, EveningAvailability = false };
        }

        public int Id { get; set; }

        public Day Monday { get; set; }

        public Day Tuesday { get; set; }

        public Day Wednesday { get; set; }

        public Day Thursday { get; set; }

        public Day Friday { get; set; }

        public Day Saturday { get; set; }

        public Day Sunday { get; set; }
    }
}
