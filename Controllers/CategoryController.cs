using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using movie_hospital_1.Reposotories.IReposotories;
using movie_hospital_1.Utilities;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace movie_hospital_1.Controllers
{
    [Authorize(Roles = $"{SD.ROLE_ADMIN},{SD.ROLE_SUPER_ADMIN}")]
    public class CategoryController : Controller
    {
        private readonly IRepossitory<Category> _categoryRepository;

        public CategoryController(IRepossitory<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // =================== INDEX ===================
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAsync(cancellationToken: cancellationToken);
            return View(categories.AsEnumerable());
        }

        // =================== CREATE ===================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                await _categoryRepository.Add(category, cancellationToken);
                await _categoryRepository.Commit(cancellationToken);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // =================== EDIT ===================
        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Category category, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                _categoryRepository.Update(category);
                await _categoryRepository.Commit(cancellationToken);
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // =================== DELETE ===================
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category = await _categoryRepository.GetOne(e => e.Id == id, cancellationToken: cancellationToken);
            if (category == null)
            {
                return NotFound();
            }

            _categoryRepository.Delete(category, cancellationToken);
            await _categoryRepository.Commit(cancellationToken);

            return RedirectToAction(nameof(Index));
        }
    }
}
