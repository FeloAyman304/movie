using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace movie_hospital_1.Reposotories.IReposotories
{
    public interface IRepossitory<T> where T : class
    {
        Task Add(T entity, CancellationToken cancellationToken = default);
        void Update(T entity);


         void Delete(T entity, CancellationToken cancellationToken);

        Task<IEnumerable<T>> GetAsync(
       Expression<Func<T, bool>>? expression = null,
       Expression<Func<T, object>>[]? includes = null,
       CancellationToken cancellationToken = default);




        Task<T?> GetOne(
                   Expression<Func<T, bool>>? expression = null,
                   Expression<Func<T, object>>[]? includes = null,
                   CancellationToken cancellationToken = default
               );
        Task Commit(CancellationToken cancellationToken);
       
    }
}
