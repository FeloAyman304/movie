using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;

namespace movie_hospital_1.Reposotories
{
    public class CategoryRepository : Repossitory<Category>, IRepossitory<Category>
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task AddRange(IEnumerable<Category> categories, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddRangeAsync(categories, cancellationToken);
        }
    }
}
