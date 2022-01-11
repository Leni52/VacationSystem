using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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

        public async Task<List<T>> All()
        {
            return await _databaseContext.Set<T>().ToListAsync();
        }

        public async Task CreateOrUpdate(T entity)
        {
            if (entity.Id != Guid.Empty)
            {
                entity.ChangeDate = DateTime.Now;
                _databaseContext.Set<T>().Update(entity);
            }
            else
            {
                await _databaseContext.AddAsync(entity);
            }
            await _databaseContext.SaveChangesAsync();
        }

        public List<T> Find(Func<T, bool> predicate)
        {
            return _databaseContext.Set<T>().Where(predicate).ToList();
        }

        public async Task<T> Get(Expression<Func<T, bool>> predicate)
        {
            return await _databaseContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public async Task<T> Get(Guid id)
        {
            return await _databaseContext.Set<T>().FirstOrDefaultAsync(e => e.Id == id);
        }
        public async Task<T> Remove(T entity)
        {
            T removed = _databaseContext.Set<T>().Remove(entity).Entity;
            await _databaseContext.SaveChangesAsync();
            return removed;
        }

    }
}
