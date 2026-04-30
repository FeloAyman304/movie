using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using System.Linq.Expressions;

namespace movie_hospital_1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly MovieRepository _movieRepository;
        private readonly OrderRepository _orderRepository;

        public OrdersController(MovieRepository movieRepository, OrderRepository orderRepository)
        {
            _movieRepository = movieRepository;
            _orderRepository = orderRepository;
        }

        // ===================== BOOK PAGE =====================
        [HttpGet]
        public async Task<IActionResult> Book(int movieId, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetOne(
                e => e.Id == movieId,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.Category,
                    m => m.Cinema
                },
                cancellationToken: cancellationToken
            );

            if (movie == null || !movie.InCinema)
                return Redirect("/Home/Index");

            return View(movie);
        }

        // ===================== ADD TO CART =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int movieId, int quantity = 1)
        {
            try
            {
                // استخدام ID ثابت بدون Login
                string userId = "guest-user-123";

                var movie = await _movieRepository.GetOne(e => e.Id == movieId);

                if (movie == null)
                {
                    return NotFound(new { success = false, message = "Movie not found" });
                }

                if (!movie.InCinema)
                {
                    return BadRequest(new { success = false, message = "Movie is not currently showing" });
                }

                Order order = new Order()
                {
                    MovieId = movieId,
                    ApplicationUserId = userId,
                    Quantity = quantity,
                    TotalPrice = movie.Price * quantity,
                    OrderDate = DateTime.Now,
                    OrderStatus = "Completed"
                };

                await _orderRepository.Add(order);
                await _orderRepository.Commit();

                return Ok(new { success = true, message = "Added to cart successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "An error occurred: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Cart(CancellationToken cancellationToken)
        {
            string userId = "guest-user-123";

            var orders = await _orderRepository.GetAsync(
                expression: e => e.ApplicationUserId == userId,
                cancellationToken: cancellationToken
            );

            foreach (var order in orders)
            {
                order.Movie = await _movieRepository.GetOne(
                    expression: m => m.Id == order.MovieId,
                    cancellationToken: cancellationToken
                );
            }

            return View(orders);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
        {
            string userId = "guest-user-123";

            var order = await _orderRepository.GetOne(
                e => e.Id == id && e.ApplicationUserId == userId,
                cancellationToken: cancellationToken
            );

            if (order != null)
            {
                _orderRepository.Delete(order, cancellationToken);
                await _orderRepository.Commit(cancellationToken);
            }

            return Redirect("/Orders/Cart");
        }
    }
}