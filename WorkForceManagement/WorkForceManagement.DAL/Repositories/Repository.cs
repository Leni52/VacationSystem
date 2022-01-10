using System;
using System.Collections.Generic;
using System.Linq;
using WorkForceManagement.DAL.Data;
using WorkForceManagement.DAL.Entities;

namespace WorkForceManagement.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : Base
    {
        private readonly DatabaseContext _databaseContext;

        public Repository(DatabaseContext context)
        {
            _databaseContext = context;
        }

        public List<T> All()
        {
            return _databaseContext.Set<T>().ToList();
        }

        public void CreateOrUpdate(T entity)
        {
            if (entity.Id != Guid.Empty)
            {
                entity.ChangeDate = DateTime.Now;
                _databaseContext.Set<T>().Update(entity);
            }
            else
            {
                _databaseContext.Add(entity);
            }
            _databaseContext.SaveChanges();
        }

        public List<T> Find(Func<T, bool> predicate)
        {
            return _databaseContext.Set<T>().Where(predicate).ToList();
        }

        public T Get(Func<T, bool> predicate)
        {
            return _databaseContext.Set<T>().FirstOrDefault(predicate);
        }

        public T Get(Guid id)
        {
            return _databaseContext.Set<T>().FirstOrDefault(e => e.Id == id);
        }
        public T Remove(T entity)
        {
            T removed = _databaseContext.Set<T>().Remove(entity).Entity;
            _databaseContext.SaveChanges();
            return removed;
        }

    }
}
