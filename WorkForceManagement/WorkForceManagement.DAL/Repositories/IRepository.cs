using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DAL.Repositories
{
    public interface IRepository<T> where T : Base
    {
        Task<T> Get(Expression<Func<T, bool>> predicate);

        Task<T> Get(Guid id);

        Task<List<T>> All();

        List<T> Find(Func<T, bool> predicate);

        Task CreateOrUpdate(T entity);

        Task<T> Remove(T entity);
        Task SaveChanges();

    }
}
