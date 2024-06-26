using Microsoft.EntityFrameworkCore;
using RestfullWithAspNet.Model.Base;
using RestfullWithAspNet.Model.Context;

namespace RestfullWithAspNet.Repository.Generic
{
    /// <summary>
    /// Generic repository implementation for CRUD operations on entities.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected MySQLContext _context;
        private DbSet<T> dataset;

        /// <summary>
        /// Initializes a new instance of the GenericRepository class.
        /// </summary>
        /// <param name="context">The database context for accessing entities.</param>
        public GenericRepository(MySQLContext context)
        {
            _context = context;
            dataset = _context.Set<T>();
        }

        /// <summary>
        /// Retrieves all entities from the repository.
        /// </summary>
        /// <returns>A list of entities.</returns>
        public List<T> FindAll()
        {
            return dataset.ToList();
        }

        /// <summary>
        /// Retrieves an entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity.</param>
        /// <returns>The entity with the specified ID, or null if not found.</returns>
        public T FindById(long id)
        {
            var result = dataset.SingleOrDefault(p => p.Id.Equals(id));
            return result ?? throw new Exception("Entity not found.");
        }

        /// <summary>
        /// Creates a new entity in the repository.
        /// </summary>
        /// <param name="item">The entity to create.</param>
        /// <returns>The created entity.</returns>
        public T Create(T item)
        {
            try
            {
                dataset.Add(item);
                _context.SaveChanges();
                return item;
            }
            catch
            {
                throw; // Preserve the original exception
            }
        }

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="item">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        public T Update(T item)
        {
            var result = dataset.SingleOrDefault(p => p.Id.Equals(item.Id));
            if (result != null)
            {
                try
                {
                    _context.Entry(result).CurrentValues.SetValues(item);
                    _context.SaveChanges();
                    return result;
                }
                catch
                {
                    throw; // Preserve the original exception
                }
            }
            else
            {
                throw new Exception("Entity not found.");
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its ID.
        /// </summary>
        /// <param name="id">The ID of the entity to delete.</param>
        public void Delete(long id)
        {
            var result = dataset.SingleOrDefault(p => p.Id.Equals(id));
            if (result != null)
            {
                try
                {
                    dataset.Remove(result);
                    _context.SaveChanges();
                }
                catch
                {
                    throw; // Preserve the original exception
                }
            }
        }

        /// <summary>
        /// Checks if an entity with the specified ID exists in the repository.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>True if the entity exists, false otherwise.</returns>
        public bool Exists(long id)
        {
            return dataset.Any(p => p.Id.Equals(id));
        }

        /// <summary>
        /// Finds entities with a paged search.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        /// <returns>A list of entities.</returns>
        public List<T> FindWithPagedSearch(string query)
        {
            return dataset.FromSqlRaw<T>(query).ToList();
        }

        /// <summary>
        /// Gets the count of entities with a paged search.
        /// </summary>
        /// <param name="query">The query to search for.</param>
        /// <returns>The count of entities.</returns>
        public int GetCount(string query)
        {
            var result = "";
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    var scalarResult = command.ExecuteScalar();
                    result = scalarResult?.ToString() ?? throw new Exception("Count not found.");
                }
            }
            return int.TryParse(result, out int count) ? count : throw new Exception("Count not found.");
        }
    }
}
