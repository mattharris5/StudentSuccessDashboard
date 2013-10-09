using System;
using System.Data.Objects.DataClasses;

namespace SSD.ViewModels.DataTables
{
    public static class RepositoryFunctions
    {
        // NOTE: At runtime, this method is captured by Linq to Entities to translate to SQL.  See: http://msdn.microsoft.com/en-us/library/dd487127.aspx
        // NOTE: This method is here just to support unit testing.  When EF context is used the implementation is ignored.
        [EdmFunction("SqlServer", "STR")]
        public static string StringConvert(double? number)
        {
            return number.ToString();
        }

        [EdmFunction("SqlServer", "DATEDIFF")]
        public static int? DateDiff(string datePartArg, DateTime? startDate, DateTime? endDate)
        {
            if (datePartArg.ToLower() == "mi")
            {
                return (int)endDate.Value.Subtract(startDate.Value).TotalMinutes;
            }
            throw new NotSupportedException("Only minutes date part is supported.");
        }
    }
}
