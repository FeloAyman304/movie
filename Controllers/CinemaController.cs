using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;
using movie_hospital_1.Utilities;

namespace movie_hospital_1.Controllers
{
    [Authorize(Roles = $"{SD.ROLE_ADMIN},{SD.ROLE_SUPER_ADMIN}")]
    public class CinemaController : Controller
    {
        private readonly IRepossitory<Cinema> _cinemaRepository;

        public CinemaController(IRepossitory<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var cinemas = await _cinemaRepository.GetAsync(cancellationToken: cancellationToken);
            return View(cinemas.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Cinema cinema, IFormFile ImageFile, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    cinema.ImageURL = "/images/cinemas/" + uniqueFileName;
                }

                await _cinemaRepository.Add(cinema, cancellationToken);
                await _cinemaRepository.Commit(cancellationToken);

                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            return cinema == null ? NotFound() : View(cinema);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile ImageFile, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var existingCinema = await _cinemaRepository.GetOne(e => e.Id == cinema.Id, cancellationToken: cancellationToken);
                if (existingCinema == null)
                    return NotFound();

                existingCinema.Name = cinema.Name;

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cinemas");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    existingCinema.ImageURL = "/images/cinemas/" + uniqueFileName;
                }

                _cinemaRepository.Update(existingCinema);
                await _cinemaRepository.Commit(cancellationToken);

                return RedirectToAction(nameof(Index));
            }

            return View(cinema);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var cinema = await _cinemaRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (cinema == null)
                return NotFound();

            _cinemaRepository.Delete(cinema, cancellationToken);
            await _cinemaRepository.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
