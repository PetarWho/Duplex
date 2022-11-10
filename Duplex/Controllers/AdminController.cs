using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}
