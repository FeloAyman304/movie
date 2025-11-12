using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using movie_hospital_1.Utilities;

namespace movie_hospital_1.Controllers
{
    [Authorize(Roles =$"{SD.ROLE_ADMIN},{SD.ROLE_SUPER_ADMIN}")] 
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
