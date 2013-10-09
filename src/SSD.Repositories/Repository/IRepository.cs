using System.Collections.Generic;
using System.Linq;

namespace SSD.Repository
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        void Remove(T item);
        void Update(T item);
        IQueryable<T> Items { get; }
    }
}
