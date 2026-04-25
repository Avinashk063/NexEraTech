using Microsoft.EntityFrameworkCore;
using NexEraTech.Infrastructure.Data;
using NexEraTech.Infrastructure.Repository.Interface;
using System.Linq.Expressions;

namespace NexEraTech.Infrastructure.Repository
{
    public class NexEraTechRepository : INexEraTechRepository
    {
        private readonly NexEraTechDB _context;
        public NexEraTechRepository(NexEraTechDB context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync<Entity>(Entity entity) where Entity : class
        {
            await _context.Set<Entity>().AddAsync(entity);
            return true;
        }

        public IQueryable<Entity> GetAll<Entity>() where Entity : class
        {
            return _context.Set<Entity>();
        }

        public IQueryable<Entity> GetMany<Entity>(Expression<Func<Entity, bool>> expression) where Entity : class
        {
            return GetAll<Entity>().Where(expression).AsQueryable();
        }

        public async Task<Entity> GetAsync<Entity>(Expression<Func<Entity, bool>> expression) where Entity : class
        {
            return await GetMany(expression).FirstOrDefaultAsync();
        }

        public async Task<Entity> GetByIdAsync<Entity>(int id) where Entity : class
        {
            return await _context.Set<Entity>().FindAsync(id);
        }

        public async Task<Entity> GetByIdAsync<Entity>(long id) where Entity : class
        {
            return await _context.Set<Entity>().FindAsync(id);
        }

        public async Task<Entity> GetByIdAsync<Entity>(string id) where Entity : class
        {
            return await _context.Set<Entity>().FindAsync(id);
        }

        public bool Update<Entity>(Entity entity) where Entity : class
        {
            _context.Entry(entity).State = EntityState.Modified;
            return true;
        }

        public bool Delete<Entity>(Entity entity) where Entity : class
        {
            _context.Set<Entity>().Remove(entity);
            return true;
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
