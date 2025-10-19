





using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;

namespace movie_hospital_1.Controllers
{
    public class ActorController : Controller
    {
        private readonly ApplicationDbContext _context = new();

        public IActionResult Index()
        {
            var actors = _context.Actors.ToList();
            return View(actors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actors");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ProfileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(stream);
                    }

                    actor.ProfilePicture = "/images/actors/" + uniqueFileName;
                }

                _context.Actors.Add(actor);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(actor);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor == null)
                return NotFound();

            return View(actor);
        }

        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile ProfileImage)
        {
            if (ModelState.IsValid)
            {
                var existingActor = _context.Actors.Find(actor.Id);
                if (existingActor == null)
                    return NotFound();

                existingActor.Name = actor.Name;
                existingActor.Bio = actor.Bio;

                if (ProfileImage != null && ProfileImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/actors");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid() + Path.GetExtension(ProfileImage.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfileImage.CopyTo(stream);
                    }

                    existingActor.ProfilePicture = "/images/actors/" + uniqueFileName;
                }

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(actor);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var actor = _context.Actors.Find(id);
            if (actor != null)
            {
                _context.Actors.Remove(actor);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
