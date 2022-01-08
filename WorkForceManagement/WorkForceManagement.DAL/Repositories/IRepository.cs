using System;
using System.Collections.Generic;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DAL.Repositories
{
    public interface IRepository<T> where T : Base
    {
        T Get(Func<T, bool> predicate);

        T Get(Guid id);

        List<T> All();

        List<T> Find(Func<T, bool> predicate);

        void CreateOrUpdate(T entity);

        public T Remove(T entity);

    }
}
