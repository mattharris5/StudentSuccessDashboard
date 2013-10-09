using Castle.MicroKernel;
using EFCachingProvider.Caching;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;

namespace SSD.Data
{
    public class DataContextConfigurator : IDataContextConfigurator
    {
        public bool EnableCaching { get; set; }

        public bool EnableTracing { get; set; }

        public void Configure(IKernel kernel, IEducationContext instance)
        {
            if (typeof(DbContext).IsAssignableFrom(instance.GetType()))
            {
                PrepareForUse((DbContext)instance);
            }
            ConfigureCache(kernel, instance);
            ConfigureTracing(instance);
        }

        private static void PrepareForUse(DbContext instance)
        {
            if (instance.Database.Exists())
            {
                instance.Database.Initialize(false);
            }
            else
            {
                string connectionString = instance.Database.Connection.ConnectionString;
                Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Initializing database for {0} at connection string '{1}'", instance.GetType().Name, connectionString), "Information");
                instance.Database.Initialize(true);
            }
        }

        private void ConfigureCache(IKernel kernel, IEducationContext instance)
        {
            if (EnableCaching)
            {
                instance.Cache = kernel.Resolve<ICache>();
                instance.CachingPolicy = CachingPolicy.CacheAll;
            }
        }

        private void ConfigureTracing(IEducationContext instance)
        {
            if (EnableTracing)
            {
                instance.Log = new TraceTextWriter();
            }
        }
    }
}
