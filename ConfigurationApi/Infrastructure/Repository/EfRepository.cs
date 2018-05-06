using ConfigurationApi.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationApi.Infrastructure.Repository
{
    public class EFRepository<T> : IRepository<T> where T : class
    {
        private readonly DbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public EFRepository(ConfigDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("dbContext can not be null.");
            _dbSet = dbContext.Set<T>();
        }

        #region IRepository Members

        public Task<IEnumerable<T>> AllAsync(string applicationName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            return _dbSet;
        }

        public async Task<T> Find(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Insert(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        async Task IRepository<T>.Delete(T entity)
        {
            EntityEntry dbEntityEntry = _dbContext.Entry(entity);

            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
        }

        async Task IRepository<T>.Update(T config)
        {
            _dbSet.Attach(config);
            _dbContext.Entry(config).State = EntityState.Modified;
        }
        #endregion
    }
}
