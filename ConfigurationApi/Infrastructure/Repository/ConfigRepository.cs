using ConfigurationApi.Infrastructure;
using ConfigurationApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationApi.Repository
{
    public class ConfigRepository : IRepository<Config>
    {
        private readonly ConfigDbContext _context;
        public ConfigRepository() => _context = new ConfigDbContext(new DbContextOptions<ConfigDbContext>());

        public async Task<IEnumerable<Config>> AllAsync(string applicationName)
        {
            var configs = await _context.Configurations.ToListAsync();
            return configs.FindAll(x => x.ApplicationName == applicationName);
        }

        public async Task<IEnumerable<Config>> AllAsync()
        {
            return await _context.Configurations.ToListAsync();
        }

        public async Task Delete(Guid id)
        {
            var entity = await Find(id);
            _context.Set<Config>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task Delete(Config entity)
        {
            throw new NotImplementedException();
        }

        public async Task<Config> Find(Guid id)
        {
            return await _context.Configurations.FirstOrDefaultAsync(x => x.Id == id);
        }

        public IEnumerable<Config> GetAll()
        {
            throw new NotImplementedException();
        }

        public async Task Insert(Config entity)
        {
            await _context.Set<Config>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task Update(Config config)
        {
            _context.Set<Config>().Update(config);
            await _context.SaveChangesAsync();
        }
    }
}
