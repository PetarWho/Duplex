using Duplex.Core.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Administration.Controllers
{
    [Area(AreaConstants.AdministrationArea)]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Panel()
        {
            return View();
        }
    }
}
