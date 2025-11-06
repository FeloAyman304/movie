using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;

namespace movie_hospital_1.Reposotories
{
    public class ActorRepository : Repossitory<Actor>, IRepossitory<Actor>
    {
        private readonly ApplicationDbContext _context;

        public ActorRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(IEnumerable<Actor> actors, CancellationToken cancellationToken = default)
        {
            await _context.Actors.AddRangeAsync(actors, cancellationToken);
        }
    }
}
