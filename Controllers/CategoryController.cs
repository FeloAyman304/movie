using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataAccess;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories;
using System.Threading;
using System.Threading.Tasks;


namespace movie_hospital_1.Controllers
{
    public class CategoryController : Controller
    {
        //private ApplicationDbContext _context = new();
          Repossitory<Category> _categoryRepossitory = new();

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepossitory.GetAsync(cancellationToken: cancellationToken);
            return View(categories.AsEnumerable());
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category,CancellationToken cancellationToken)
        {
            if (ModelState is not null)
            {
               await _categoryRepossitory.Add(category, cancellationToken);
               await _categoryRepossitory.Commit(cancellationToken);
                return RedirectToAction("Index");
            }

            return View(category);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id ,CancellationToken cancellationToken)
        {
            var category = await _categoryRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            return category == null ? NotFound() : View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category,CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
             _categoryRepossitory.Update(category);
                _categoryRepossitory.Commit(cancellationToken);
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
     

        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepossitory.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }
            _categoryRepossitory.Delete(category, cancellationToken);
            await _categoryRepossitory.Commit(cancellationToken);
            return RedirectToAction("Index");
        }

    }
}
