using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;

namespace movie_hospital_1.Reposotories
{
    public class CinemaRepository : Repossitory<Cinema>, IRepossitory<Cinema>
    {
        private readonly ApplicationDbContext _context;

        public CinemaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(IEnumerable<Cinema> cinemas, CancellationToken cancellationToken = default)
        {
            await _context.Cinemas.AddRangeAsync(cinemas, cancellationToken);
        }
    }
}
