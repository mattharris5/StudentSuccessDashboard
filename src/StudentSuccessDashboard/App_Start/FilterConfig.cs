using SSD.ActionFilters;
using System;
using System.Web.Mvc;

namespace SSD
{
    public static class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException("filters");
            }
            filters.Add(new HandleEntityErrorAttribute());
            filters.Add(new TraceActionAttribute());
            filters.Add(new UserIdentityMapAttribute { Order = 10 });
        }
    }
}