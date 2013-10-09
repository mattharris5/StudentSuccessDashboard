using SSD.Domain;

namespace SSD.Repository
{
    public interface IServiceOfferingRepository : IRepository<ServiceOffering>
    {
        void AddLink(ServiceOffering serviceOffering, User user);
        void DeleteLink(ServiceOffering serviceOffering, User user);
    }
}
