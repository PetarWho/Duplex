using Duplex.Core.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Errors.Controllers
{
    [Area(AreaConstants.ErrorsArea)]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult _404()
        {
            return View("404");
        }

        [HttpGet]
        public IActionResult _403()
        {
            return View("403");
        }
        [HttpGet]
        public IActionResult _402()
        {
            return View("402");
        }
        [HttpGet]
        public IActionResult _401()
        {
            return View("401");
        }
    }
}
