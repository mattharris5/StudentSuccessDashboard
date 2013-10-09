using SSD.Domain;

namespace SSD.Repository
{
    public interface IServiceTypeRepository : IRepository<ServiceType>
    {
        void AddLink(ServiceType serviceType, Category category);
        void DeleteLink(ServiceType serviceType, Category category);
    }
}
