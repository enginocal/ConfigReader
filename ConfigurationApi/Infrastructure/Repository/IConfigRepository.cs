using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationApi.Repository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        Task<IEnumerable<T>> AllAsync(string applicationName);
        Task<T> Find(Guid id);
        Task Insert(T entity);
        Task Delete(T entity);
        Task Update(T entity);
    }
}
