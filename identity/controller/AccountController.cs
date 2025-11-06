using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.dataModel;
using System.Threading.Tasks;

namespace movie_hospital_1.identity.controller
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager
            )
        {
            _userManager = userManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
                return View(registerVM);
            var result = await _userManager.CreateAsync(new()
            {
                firstName = registerVM.firstName,
                lastName = registerVM.lastName,
                Email = registerVM.Email,
                UserName = registerVM.userName,
            }, registerVM.password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Code);
                }
                return View(registerVM);
            }
            return RedirectToAction("Login");
        }
    }
}
