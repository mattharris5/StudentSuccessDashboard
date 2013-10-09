using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;

namespace SSD.Data
{
    public static class DbContextExtensions
    {
        public static ObjectContext AsObjectContext(this DbContext context)
        {
            return ((IObjectContextAdapter)context).ObjectContext;
        }
    }
}
