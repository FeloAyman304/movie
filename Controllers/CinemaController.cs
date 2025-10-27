using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;

namespace movie_hospital_1.Controllers
{
    public class CinemaController : Controller
    {
        Repossitory<Cinema> _CinemaRepossitory = new();

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _CinemaRepossitory.GetAsync(cancellationToken: cancellationToken);
            return View(categories.AsEnumerable());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema, IFormFile ImageFile,CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
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

                    
                    cinema.ImageURL = "/images/" + uniqueFileName;
                }

                _CinemaRepossitory.Add(cinema,cancellationToken);
                _CinemaRepossitory.Commit(cancellationToken);
                return RedirectToAction(nameof(Index));
            }
            return View(cinema);
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor = await _CinemaRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            return actor == null ? NotFound() : View(actor);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile ImageFile, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var existingCinema = await _CinemaRepossitory.GetOne(e => e.Id == cinema.Id, cancellationToken: cancellationToken);
                if (existingCinema == null)
                    return NotFound();

                existingCinema.Name = cinema.Name;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    existingCinema.ImageURL = "/images/" + uniqueFileName;
                }

                _CinemaRepossitory.Update(existingCinema);
                await _CinemaRepossitory.Commit(cancellationToken);

                return RedirectToAction("Index");
            }
            return View(cinema);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await _CinemaRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (cinema == null)
                return NotFound();

            _CinemaRepossitory.Delete(cinema, cancellationToken);
            await _CinemaRepossitory.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
