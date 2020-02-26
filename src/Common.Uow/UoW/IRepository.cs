using System;
using System.Linq;

namespace Common.UoW
{
    public interface IRepository<T> : IReadRepository<T>, IDisposable where T : class
    {
        void Add(T entity);
        void Delete(T entity);
        void Delete(object id);
        void Update(T entity);
    }

    public interface IReadRepository<out T> where T : class
    {
        IQueryable<T> Query();
    }
}
