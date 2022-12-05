using Microsoft.AspNetCore.Mvc;
using Duplex.Core.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace Duplex.Areas.Administration.Controllers
{
    [Area(AreaConstants.AdministrationArea)]
    [Authorize(Roles = "Admin")]
    public class ManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
