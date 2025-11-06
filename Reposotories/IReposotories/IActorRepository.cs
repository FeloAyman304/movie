using Microsoft.EntityFrameworkCore;

namespace movie_hospital_1.Reposotories.IReposotories
{
    public interface IActorRepository<T> where T : class
    {
        Task AddRange(IEnumerable<ActorRepository> actors, CancellationToken cancellationToken = default);
      
    }
}
