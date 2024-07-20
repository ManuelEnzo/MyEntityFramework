using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyEntityFramework
{
    public interface ICommonAPI<T> where T : class
    {
        /// <summary>
        /// Gets an entity by ID.
        /// </summary>
        /// <param name="id">Entity ID.</param>
        /// <returns>Found entity.</returns>
        Task<T> GetByIdAsync(int id);

        /// <summary>
        /// Gets an entity that satisfies the specified keys.
        /// </summary>
        /// <param name="keys">Unique entity keys.</param>
        /// <returns>Found entity.</returns>
        Task<T> GetByKeysAsync(Dictionary<string, object> keys);

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>Collection of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets all entities that satisfy the specified keys.
        /// </summary>
        /// <param name="keys">Dictionary of keys and values.</param>
        /// <returns>Collection of found entities.</returns>
        Task<IEnumerable<T>> GetAllAsync(Dictionary<string, object> keys);

        /// <summary>
        /// Finds entities that satisfy a predicate.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <returns>Collection of found entities.</returns>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Checks if an entity that satisfies a predicate exists.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <returns>True if exists, otherwise False.</returns>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Counts the number of entities that satisfy a predicate.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <returns>Number of found entities.</returns>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets a single entity that satisfies a predicate or null if not found.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <returns>Found entity or null.</returns>
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds a set of entities to the database.
        /// </summary>
        /// <param name="entities">Entities to add.</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates an entity in the database.
        /// </summary>
        /// <param name="entity">Entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Updates a set of entities in the database.
        /// </summary>
        /// <param name="entities">Entities to update.</param>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">Entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Removes a set of entities from the database.
        /// </summary>
        /// <param name="entities">Entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Saves the changes made to the database context.
        /// </summary>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveChangesAsync();

        /// <summary>
        /// Alias for SaveChangesAsync.
        /// </summary>
        /// <returns>Number of state entries written to the database.</returns>
        Task<int> SaveAsync();

        /// <summary>
        /// Alias for AddAsync.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        Task InsertAsync(T entity);

        /// <summary>
        /// Alias for AddRangeAsync.
        /// </summary>
        /// <param name="entities">Entities to insert.</param>
        Task InsertRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Updates the entity with new values.
        /// </summary>
        /// <param name="entity">Entity to modify.</param>
        /// <param name="newEntity">Entity with new values.</param>
        void SetNewValuesFromEntity(T entity, object newEntity);

        /// <summary>
        /// Returns the first entity that matches the predicate or default.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <returns>Found entity or default.</returns>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Gets the maximum value of a property for entities that satisfy the predicate.
        /// </summary>
        /// <param name="predicate">Lambda expression to filter entities.</param>
        /// <param name="selector">Lambda expression to select the property.</param>
        /// <returns>Maximum value of the selected property.</returns>
        Task<TResult> MaxAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector);

        /// <summary>
        /// Gets the maximum value of the specified Id property.
        /// </summary>
        /// <param name="idPropertyName">Name of the Id property.</param>
        /// <returns>Maximum value of the specified Id property.</returns>
        Task<int> GetMaxIdAsync(string idPropertyName);
    }

    public class CommonAPI<T> : ICommonAPI<T> where T : class
    {
        private readonly DbContext _dbContext;

        public CommonAPI(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public virtual async Task<T> GetByIdAsync(int id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().AnyAsync(predicate);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().CountAsync(predicate);
        }

        public virtual async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().SingleOrDefaultAsync(predicate);
        }

        public virtual async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public virtual async Task<int> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<int> SaveAsync()
        {
            return await SaveChangesAsync();
        }

        public virtual async Task InsertAsync(T entity)
        {
            await AddAsync(entity);
        }

        public virtual async Task InsertRangeAsync(IEnumerable<T> entities)
        {
            await AddRangeAsync(entities);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(Dictionary<string, object> keys)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var key in keys)
            {
                var propertyInfo = typeof(T).GetProperty(key.Key);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Key '{key.Key}' not found in entity '{typeof(T).Name}'");
                }

                var parameter = Expression.Parameter(typeof(T), "x");
                var member = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(key.Value);

                Expression body;
                if (key.Value == null)
                {
                    // Handle null values in the key
                    body = Expression.Equal(member, Expression.Constant(null, propertyInfo.PropertyType));
                }
                else
                {
                    // Handle non-null values
                    body = Expression.Equal(member, constant);
                }

                var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
                query = query.Where(predicate);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<T> GetByKeysAsync(Dictionary<string, object> keys)
        {
            IQueryable<T> query = _dbContext.Set<T>();

            foreach (var key in keys)
            {
                var propertyInfo = typeof(T).GetProperty(key.Key);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Key '{key.Key}' not found in entity '{typeof(T).Name}'");
                }

                var parameter = Expression.Parameter(typeof(T), "x");
                var member = Expression.Property(parameter, propertyInfo);
                var constant = Expression.Constant(key.Value);

                Expression body;
                if (key.Value == null)
                {
                    // Handle null values in the key
                    body = Expression.Equal(member, Expression.Constant(null, propertyInfo.PropertyType));
                }
                else
                {
                    // Handle non-null values
                    body = Expression.Equal(member, constant);
                }

                var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);
                query = query.Where(predicate);
            }

            return await query.FirstOrDefaultAsync();
        }

        public virtual void SetNewValuesFromEntity(T entity, object newEntity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (newEntity == null)
                throw new ArgumentNullException(nameof(newEntity));

            var entityEntry = _dbContext.Entry(entity);

            foreach (var property in entityEntry.CurrentValues.Properties)
            {
                var newValue = newEntity.GetType().GetProperty(property.Name)?.GetValue(newEntity, null);
                if (newValue != null)
                {
                    entityEntry.CurrentValues[property] = newValue;
                }
            }

            _dbContext.SaveChanges();
        }

        public virtual async Task<TResult> MaxAsync<TResult>(Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> selector)
        {
            return await _dbContext.Set<T>().Where(predicate).MaxAsync(selector);
        }
        public virtual async Task<int> GetMaxIdAsync(string idPropertyName)
        {
            if (string.IsNullOrWhiteSpace(idPropertyName))
            {
                throw new ArgumentException("ID property name must be provided.", nameof(idPropertyName));
            }

            return await _dbContext.Set<T>().MaxAsync(e => EF.Property<int>(e, idPropertyName));
        }
    }

}
