using Microsoft.EntityFrameworkCore;

namespace movie_hospital_1.Reposotories.IReposotories
{
    public interface ICategoryRepository<T> where T : class
    {
    Task AddRange(IEnumerable<CategoryRepository> categories, CancellationToken cancellationToken = default);
      
    }
}
