using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;

namespace movie_hospital_1.Reposotories
{
    public class MovieRepository : Repossitory<Movie>, IRepossitory<Movie>
    {
        private readonly ApplicationDbContext _context;

        public MovieRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(IEnumerable<Movie> movies, CancellationToken cancellationToken = default)
        {
            await _context.AddRangeAsync(movies, cancellationToken);
        }
    }
}
