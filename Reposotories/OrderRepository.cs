using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
namespace movie_hospital_1.Reposotories
{
    public class OrderRepository : Repossitory<Order>
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        
    }
}