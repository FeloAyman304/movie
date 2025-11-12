



using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Models;
using movie_hospital_1.Reposotories;
using System.Diagnostics;
using System.Linq.Expressions;

namespace movie_hospital_1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MovieRepository _MovieRepository;
        private readonly CategoryRepository _CategoryRepository;
        private readonly CinemaRepository _CinemaRepository;
        private readonly ActorRepository _ActorRepository;

        public HomeController(
            ILogger<HomeController> logger,
            MovieRepository movieRepository,
            CategoryRepository categoryRepository,
            CinemaRepository cinemaRepository,
            ActorRepository actorRepository
        )
        {
            _logger = logger;
            _MovieRepository = movieRepository;
            _CategoryRepository = categoryRepository;
            _CinemaRepository = cinemaRepository;
            _ActorRepository = actorRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int? categoryId, int? year, int pageNumber = 1, CancellationToken cancellationToken = default)
        {
            int pageSize = 6;

            var allMovies = await _MovieRepository.GetAsync(
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.Category,
                    m => m.Cinema,
                    m => m.MovieActors
                },
                cancellationToken: cancellationToken
            );

            IEnumerable<Movie> filteredMovies = allMovies;

            if (!string.IsNullOrWhiteSpace(search))
                filteredMovies = filteredMovies.Where(m => m.Title.ToLower().Contains(search.ToLower()));

            if (categoryId.HasValue)
                filteredMovies = filteredMovies.Where(m => m.CategoryId == categoryId.Value);
            int totalItems = filteredMovies.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages && totalPages > 0) pageNumber = totalPages;

            var pagedMovies = filteredMovies
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            foreach (var movie in pagedMovies)
            {
                foreach (var ma in movie.MovieActors)
                {
                    ma.Actor = await _ActorRepository.GetOne(
                        a => a.Id == ma.ActorId,
                        cancellationToken: cancellationToken
                    );
                }
            }

            ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Search = search;
            ViewBag.CategoryId = categoryId;
            ViewBag.Year = year;
            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = totalPages;

            return View(pagedMovies);
        }

        public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
        {
            var movie = await _MovieRepository.GetOne(
                e => e.Id == id,
                includes: new Expression<Func<Movie, object>>[]
                {
                    e => e.Category,
                    e => e.Cinema,
                    e => e.MovieActors
                },
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

            foreach (var ma in movie.MovieActors)
            {
                ma.Actor = await _ActorRepository.GetOne(
                    a => a.Id == ma.ActorId,
                    cancellationToken: cancellationToken
                );
            }

            return View(movie);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
