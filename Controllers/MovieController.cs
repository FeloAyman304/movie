using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using System.Linq.Expressions;

namespace movie_hospital_1.Controllers
{
    public class MovieController : Controller
    {
        Repossitory<Movie> _MovieRepository = new(); 
        Repossitory<Category> _CategoryRepository = new();
        Repossitory<Cinema> _CinemaRepository = new();
        Repossitory<Actor> _ActorRepository = new();
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

                // ربط الممثلين بالفيلم
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
            var cinema = await _MovieRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (cinema == null)
                return NotFound();

            _MovieRepository.Delete(cinema, cancellationToken);
            await _MovieRepository.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            // جلب الفيلم مع MovieActors فقط
            var movie = await _MovieRepository.GetOne(
                e => e.Id == id,
                includes: new Expression<Func<Movie, object>>[] { e => e.MovieActors },
                cancellationToken: cancellationToken
            );

            if (movie == null)
                return NotFound();

            // جلب كل الـActors المرتبطين لكل MovieActor
            if (movie.MovieActors != null && movie.MovieActors.Any())
            {
                foreach (var ma in movie.MovieActors)
                {
                    if (ma != null)
                        ma.Actor = await _ActorRepository.GetOne(a => a.Id == ma.ActorId, cancellationToken: cancellationToken);
                }
            }

            // جلب القوائم المنسدلة
            ViewBag.Categories = await _CategoryRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Cinemas = await _CinemaRepository.GetAsync(cancellationToken: cancellationToken);
            ViewBag.Actors = await _ActorRepository.GetAsync(cancellationToken: cancellationToken);

            return View(movie);
        }

        // ----------------- EDIT POST -----------------
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

            // جلب الفيلم الحالي مع MovieActors
            var existingMovie = await _MovieRepository.GetOne(
                e => e.Id == movie.Id,
                includes: new Expression<Func<Movie, object>>[] { e => e.MovieActors },
                cancellationToken: cancellationToken
            );

            if (existingMovie == null)
                return NotFound();

            // تعديل البيانات الأساسية
            existingMovie.Title = movie.Title;
            existingMovie.Description = movie.Description;
            existingMovie.CategoryId = movie.CategoryId;
            existingMovie.CinemaId = movie.CinemaId;

            // رفع الصورة الجديدة إذا موجودة
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

            // تعديل الممثلين المرتبطين بطريقة صحيحة
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
