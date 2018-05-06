using ConfigurationApi.Infrastructure.Repository;
using ConfigurationApi.Repository;
using System;
using System.Transactions;

namespace ConfigurationApi.Infrastructure
{
    public class EfUnitOfWork : IUnitOfWork
    {
        private bool _disposed = false;
        private readonly ConfigDbContext _dbContext;

        public EfUnitOfWork(ConfigDbContext dbContext)
        {
            //Database.SetInitializer<ConfigDbContext>(null);

            if (dbContext == null)
                throw new ArgumentNullException("dbContext can not be null.");

            _dbContext = dbContext;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            return new EFRepository<T>(_dbContext);
        }

        public int SaveChanges()
        {
            try
            {
                using (var transaction = _dbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var result = _dbContext.SaveChanges();
                        transaction.Commit();
                        return result;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
