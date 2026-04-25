using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NexEraTech.Infrastructure.Repository.Interface
{
    public interface INexEraTechRepository
    {
        /// <summary>
        /// Add an entity to the table
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync<Entity>(Entity entity) where Entity : class;

        /// <summary>
        /// Gets all records for the specified entity. No mapping executed.
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        IQueryable<Entity> GetAll<Entity>() where Entity : class;

        /// <summary>
        /// Get all records that meet the given expression
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="expression">Expression to filter on</param>
        /// <returns></returns>
        IQueryable<Entity> GetMany<Entity>(Expression<Func<Entity, bool>> expression) where Entity : class;

        /// <summary>
        /// Get first entity based off the given expression
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="expression">Expression to filter on</param>
        /// <returns></returns>
        Task<Entity> GetAsync<Entity>(Expression<Func<Entity, bool>> expression) where Entity : class;

        /// <summary>
        /// Get an entity for the given id
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> GetByIdAsync<Entity>(int id) where Entity : class;

        /// <summary>
        /// Get an entity for the given id
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> GetByIdAsync<Entity>(long id) where Entity : class;

        /// <summary>
        /// Get an entity for the given id
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Entity> GetByIdAsync<Entity>(string id) where Entity : class;

        /// <summary>
        /// Update an entity in the context
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Update<Entity>(Entity entity) where Entity : class;

        /// <summary>
        /// Delete an entity from the context
        /// </summary>
        /// <typeparam name="Entity">The entity framework entity.</typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete<Entity>(Entity entity) where Entity : class;

        /// <summary>
        /// Save Changes
        /// </summary>
        Task SaveAsync();

    }
}
