using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
