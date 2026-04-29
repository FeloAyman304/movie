using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace movie_hospital_1.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
