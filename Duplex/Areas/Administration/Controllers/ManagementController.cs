using Microsoft.AspNetCore.Mvc;
using Duplex.Core.Common.Constants;

namespace Duplex.Areas.Administration.Controllers
{
    public class ManagementController : Controller
    {
        [Area(AreaConstants.AdministrationArea)]
        public IActionResult Index()
        {
            return View();
        }
    }
}
