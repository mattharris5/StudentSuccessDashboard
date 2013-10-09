using System;

namespace SSD.Repository
{
    public interface IRepositoryContainer
    {
        TRepository Obtain<TRepository>() where TRepository : class;
        void Save();
    }
}
