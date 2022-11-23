using Duplex.Core.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Errors.Controllers
{
    [Area(AreaConstants.ErrorsArea)]
    public class ErrorController : Controller
    {
        [HttpGet]
        public IActionResult NotFound404()
        {
            return View();
        }
    }
}
