

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using movie_hospital_1.Utilities;
using System.Linq.Expressions;

namespace movie_hospital_1.Controllers
{
    [Authorize(Roles = $"{SD.ROLE_ADMIN},{SD.ROLE_SUPER_ADMIN}")]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieRepository _MovieRepository;
        private readonly CategoryRepository _CategoryRepository;
        private readonly CinemaRepository _CinemaRepository;
        private readonly ActorRepository _ActorRepository;

        public MovieController(
            ApplicationDbContext context,
            MovieRepository movieRepository,
            CategoryRepository categoryRepository,
            CinemaRepository cinemaRepository,
            ActorRepository actorRepository)
        {
            _context = context;
            _MovieRepository = movieRepository;
            _CategoryRepository = categoryRepository;
            _CinemaRepository = cinemaRepository;
            _ActorRepository = actorRepository;
        }

   
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var movies = await _MovieRepository.GetAsync(
                includes: new Expression<Func<Movie, object>>[]
                {
                    m => m.Category,
                    m => m.Cinema,
                    m => m.MovieActors
                },
                cancellationToken: cancellationToken
            );

            foreach (var movie in movies)
            {
                if (movie.MovieActors != null && movie.MovieActors.Any())
                {
                    foreach (var ma in movie.MovieActors)
                    {
                        ma.Actor = await _ActorRepository.GetOne(a => a.Id == ma.ActorId, cancellationToken: cancellationToken);
                    }
                }
            }

            return View(movies);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Cinemas = await _CinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Actors = await _ActorRepository.GetAsync(cancellationToken: cancellationToken);
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageFile, List<int> selectedActors, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    movie.ImageURL = "/images/" + uniqueFileName;
                }

                if (selectedActors != null && selectedActors.Count > 0)
                {
                    movie.MovieActors = selectedActors.Select(aid => new MovieActor
                    {
                        ActorId = aid
                    }).ToList();
                }

                await _MovieRepository.Add(movie, cancellationToken);
                await _MovieRepository.Commit(cancellationToken);

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Cinemas = await _CinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Actors = await _ActorRepository.GetAsync(cancellationToken: cancellationToken);
            return View(movie);
        }


        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var movie = await _MovieRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (movie == null)
                return NotFound();

            _MovieRepository.Delete(movie, cancellationToken);
            await _MovieRepository.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

     
  
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var movie = await _MovieRepository.GetOne(
                e => e.Id == id,
               
                includes: new Expression<Func<Movie, object>>[]
                {
                    e => e.MovieActors,
                    e => e.Category,
                    e => e.Cinema
                },
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

       
            if (movie.MovieActors != null && movie.MovieActors.Any())
            {
                foreach (var ma in movie.MovieActors)
                {
                
                    if (ma != null)
                        ma.Actor = await _ActorRepository.GetOne(a => a.Id == ma.ActorId, cancellationToken: cancellationToken);
                }
            }
            
            ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Cinemas = await _CinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Actors = await _ActorRepository.GetAsync(cancellationToken: cancellationToken);

            return View(movie);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Movie movie, IFormFile ImageFile, List<int> selectedActors, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
         
                ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
                ViewBag.Cinemas = await _CinemaRepository.GetAsync(cancellationToken: cancellationToken);
                ViewBag.Actors = await _ActorRepository.GetAsync(cancellationToken: cancellationToken);
                return View(movie);
            }

         
            var existingMovie = await _MovieRepository.GetOne(
                e => e.Id == movie.Id,
                includes: new Expression<Func<Movie, object>>[] { e => e.MovieActors },
                cancellationToken: cancellationToken
            );

            if (existingMovie == null)
                return NotFound();

           
            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.CategoryId = movie.CategoryId;
            existingMovie.CinemaId = movie.CinemaId;
            existingMovie.Price = movie.Price;   
            existingMovie.InCinema = movie.InCinema;    

           
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await ImageFile.CopyToAsync(stream, cancellationToken);

                existingMovie.ImageURL = "/images/" + uniqueFileName;
            }

            
            if (existingMovie.MovieActors != null && existingMovie.MovieActors.Any())
            {
              
                _context.RemoveRange(existingMovie.MovieActors);
            }

            existingMovie.MovieActors.Clear();
            if (selectedActors != null && selectedActors.Count > 0)
            {
                foreach (var aid in selectedActors)
                {
                    existingMovie.MovieActors.Add(new MovieActor
                    {
                        MovieId = existingMovie.Id,
                        ActorId = aid
                    });
                }
            }

        
            await _MovieRepository.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}