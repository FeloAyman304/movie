using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;

namespace movie_hospital_1.Controllers
{
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var movies = _context.Movies
                .Include(e => e.Category)
                .Include(e => e.Cinema)
                .Include(e => e.MovieActors)
                    .ThenInclude(e => e.Actor)
                .ToList();

            return View(movies);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.Actors = _context.Actors.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImageFile, List<int> selectedActors)
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
                        ImageFile.CopyTo(stream);
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

                _context.Movies.Add(movie);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.Actors = _context.Actors.ToList();
            return View(movie);
        }

        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.Find(id);
            if (movie == null) return NotFound();

            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var movie = _context.Movies
                .Include(e => e.MovieActors)
                .ThenInclude(e => e.Actor)
                .FirstOrDefault(e => e.Id == id);

            if (movie == null) return NotFound();

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.Actors = _context.Actors.ToList();

            return View(movie);
        }

        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile ImageFile, List<int> selectedActors)
        {
            if (ModelState.IsValid)
            {
                var existingMovie = _context.Movies
                    .Include(m => m.MovieActors)
                    .FirstOrDefault(m => m.Id == movie.Id);

                if (existingMovie == null) return NotFound();

                existingMovie.Title = movie.Title;
                existingMovie.Description = movie.Description;
                existingMovie.CategoryId = movie.CategoryId;
                existingMovie.CinemaId = movie.CinemaId;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ImageFile.CopyTo(stream);
                    }

                    existingMovie.ImageURL = "/images/" + uniqueFileName;
                }

                // ✅ تحديث الممثلين
                existingMovie.MovieActors.Clear();
                if (selectedActors != null && selectedActors.Count > 0)
                {
                    existingMovie.MovieActors = selectedActors.Select(aid => new MovieActor
                    {
                        MovieId = existingMovie.Id,
                        ActorId = aid
                    }).ToList();
                }

                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = _context.Categories.ToList();
            ViewBag.Cinemas = _context.Cinemas.ToList();
            ViewBag.Actors = _context.Actors.ToList();
            return View(movie);
        }
    }
}
