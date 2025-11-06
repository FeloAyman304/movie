using Microsoft.EntityFrameworkCore;

namespace movie_hospital_1.Reposotories.IReposotories
{
    public interface ICinemaRepository<T> where T : class
    {
        Task AddRange(IEnumerable<CinemaRepository> cinemas, CancellationToken cancellationToken = default);
      
    }
}
