using SSD.Domain;

namespace SSD.Repository
{
    public interface IUserRoleRepository : IRepository<UserRole>
    {
        void AddLink(UserRole userRole, Provider provider);
        void AddLink(UserRole userRole, School school);
        void DeleteLink(UserRole userRole, Provider provider);
        void DeleteLink(UserRole userRole, School school);
    }
}
