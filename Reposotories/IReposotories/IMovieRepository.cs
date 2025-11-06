using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataModel;

namespace movie_hospital_1.Reposotories.IReposotories
{
    public interface IMovieRepository<T> where T : class
    {
        Task AddRange(IEnumerable<Movie> movies, CancellationToken cancellationToken = default);
       
    }
}
