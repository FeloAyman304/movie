





using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using movie_hospital_1.Reposotories;


namespace movie_hospital_1.Controllers
{
    public class ActorController : Controller
    {
        Repossitory<Actor> _ActorRepossitory = new();
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _ActorRepossitory.GetAsync(cancellationToken: cancellationToken);
            return View(categories.AsEnumerable());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Create(Actor actor, IFormFile ProfileImage, CancellationToken cancellationToken)
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
                        await ProfileImage.CopyToAsync(stream);
                    }

                    actor.ProfilePicture = "/images/actors/" + uniqueFileName;
                }

                await _ActorRepossitory.Add(actor, cancellationToken);
                await _ActorRepossitory.Commit(cancellationToken);

                return RedirectToAction("Index");
            }

            return View(actor);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var actor = await _ActorRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            return actor == null ? NotFound() : View(actor);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile ProfileImage, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var existingActor = await _ActorRepossitory.GetOne(e => e.Id == actor.Id, cancellationToken: cancellationToken);
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
                        await ProfileImage.CopyToAsync(stream);
                    }

                    existingActor.ProfilePicture = "/images/actors/" + uniqueFileName;
                }

               
                _ActorRepossitory.Update(existingActor);
                await _ActorRepossitory.Commit(cancellationToken);

                return RedirectToAction("Index");
            }

            return View(actor);
        }


        [HttpGet]

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category = await _ActorRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }
            _ActorRepossitory.Delete(category, cancellationToken);
            await _ActorRepossitory.Commit(cancellationToken);
            return RedirectToAction("Index");
        }
    }
}
