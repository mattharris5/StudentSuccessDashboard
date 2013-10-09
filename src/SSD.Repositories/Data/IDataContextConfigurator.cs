using Castle.MicroKernel;

namespace SSD.Data
{
    public interface IDataContextConfigurator
    {
        bool EnableCaching { get; set; }
        bool EnableTracing { get; set; }

        void Configure(IKernel kernel, IEducationContext instance);
    }
}
