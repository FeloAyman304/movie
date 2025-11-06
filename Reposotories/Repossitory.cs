using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataAccess;
using movie_hospital_1.Reposotories.IReposotories;
using System.Linq.Expressions;

namespace movie_hospital_1.Reposotories
{
    public class Repossitory<T> : IRepossitory<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public Repossitory(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>(); // ✅ دي أهم حاجة
        }

        public async Task Add(T entity, CancellationToken cancellationToken = default)
        {
            await _db.AddAsync(entity, cancellationToken);
        }

        public void Update(T entity)
        {
            _db.Update(entity);
        }

        public void Delete(T entity, CancellationToken cancellationToken)
        {
            _db.Remove(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default)
        {
            var entities = _db.AsQueryable();

            if (expression != null)
                entities = entities.Where(expression);

            if (includes is not null)
            {
                foreach (var include in includes)
                    entities = entities.Include(include);
            }

            return await entities.ToListAsync(cancellationToken);
        }

        public async Task<T?> GetOne(
            Expression<Func<T, bool>>? expression = null,
            Expression<Func<T, object>>[]? includes = null,
            CancellationToken cancellationToken = default)
        {
            return (await GetAsync(expression, includes, cancellationToken)).FirstOrDefault();
        }

        public async Task Commit(CancellationToken cancellationToken)
        {
            try
            {
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
