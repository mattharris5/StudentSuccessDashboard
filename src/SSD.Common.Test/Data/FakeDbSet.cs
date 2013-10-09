using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace SSD.Data
{
    public class FakeDbSet<T> : IDbSet<T> where T : class, new()
    {
        private HashSet<T> _SetData;
        private IQueryable _SetQuery;

        public FakeDbSet()
        {
            _SetData = new HashSet<T>();
            _SetQuery = _SetData.AsQueryable();
        }

        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find.  Alternately, mock the call to Find rather than using this fake IDbSet<T> class.");
        }

        public T Create()
        {
            return new T();
        }

        public T Add(T item)
        {
            _SetData.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _SetData.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            _SetData.Add(item);
            return item;
        }

        public bool Detach(T item)
        {
            return _SetData.Remove(item);
        }

        Type IQueryable.ElementType
        {
            get { return _SetQuery.ElementType; }
        }

        Expression IQueryable.Expression
        {
            get { return _SetQuery.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _SetQuery.Provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _SetData.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _SetData.GetEnumerator();
        }

        public virtual TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> to override Create<TDerivedEntity>.  Alternately, mock the call to Create<TDerivedEntity> rather than using this fake IDbSet<T> class.");
        }

        public virtual ObservableCollection<T> Local
        {
            get { throw new NotImplementedException("Derive from FakeDbSet<T> to override Local.  Alternately, mock the call to Local rather than using this fake IDbSet<T> class."); }
        }
    }
}
