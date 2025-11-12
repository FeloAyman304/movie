using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using System.Security.Claims;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace movie_hospital_1.Controllers
{
    [Authorize] // يتطلب تسجيل الدخول للحجز
    public class OrdersController : Controller
    {
        private readonly MovieRepository _MovieRepository;
        private readonly OrderRepository _OrderRepository;

        public OrdersController(MovieRepository movieRepository, OrderRepository orderRepository)
        {
            _MovieRepository = movieRepository;
            _OrderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Book(int movieId, CancellationToken cancellationToken)
        {
            var movie = await _MovieRepository.GetOne(
                e => e.Id == movieId,
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.Category,
                    m => m.Cinema
                },
                cancellationToken: cancellationToken
            );

            if (movie == null || !movie.InCinema)
            {
                TempData["ErrorMessage"] = "The movie is not available for booking.";
                return RedirectToAction("Index", "Home");
            }

            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int movieId, int quantity = 1, CancellationToken cancellationToken = default)
        {
            if (quantity <= 0)
            {
                TempData["ErrorMessage"] = "Quantity must be greater than zero.";
                return RedirectToAction(nameof(Book), new { movieId = movieId });
            }

            var movie = await _MovieRepository.GetOne(e => e.Id == movieId, cancellationToken: cancellationToken);
            if (movie == null)
            {
                TempData["ErrorMessage"] = "The selected movie was not found.";
                return RedirectToAction("Index", "Home");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var order = new Order
            {
                MovieId = movieId,
                ApplicationUserId = userId,
                Quantity = quantity,
                TotalPrice = movie.Price * quantity,
                OrderStatus = "Completed",
                OrderDate = DateTime.Now
            };

            await _OrderRepository.Add(order, cancellationToken);
            await _OrderRepository.Commit(cancellationToken);

            TempData["SuccessMessage"] = $"Your booking for '{movie.Title}' has been placed successfully! Total: {order.TotalPrice.ToString("C")}";

            return RedirectToAction(nameof(UserOrders));
        }

     
        public async Task<IActionResult> UserOrders(CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var userOrders = await _OrderRepository.GetAsync(
                        o => o.ApplicationUserId == userId,
                                     includes: new Expression<Func<Order, object>>[] { o => o.Movie },
                cancellationToken: cancellationToken
            );

            return View(userOrders);
        }
        // -------------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            var order = await _OrderRepository.GetOne(
                e => e.Id == id && e.ApplicationUserId == userId,
                cancellationToken: cancellationToken
            );

            if (order == null)
            {
                TempData["ErrorMessage"] = "Order not found or you do not have permission to delete it.";
                return RedirectToAction(nameof(UserOrders));
            }

            _OrderRepository.Delete(order, cancellationToken);
            await _OrderRepository.Commit(cancellationToken);

            TempData["SuccessMessage"] = "Booking successfully deleted.";

            return RedirectToAction(nameof(UserOrders));
        }
    }
}