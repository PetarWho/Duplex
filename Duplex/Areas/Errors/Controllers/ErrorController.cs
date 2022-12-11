using Duplex.Core.Common.Constants;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Errors.Controllers
{
    [Area(AreaConstants.ErrorsArea)]
    public class ErrorController : Controller
    {
        public IActionResult _404() => View("404");
        public IActionResult _403() => View("403");
        public IActionResult _402() => View("402");
        public IActionResult _401() => View("401");
        public IActionResult _400() => View("400");
        public IActionResult _502() => View("502");
        public IActionResult InvalidSummoner(string message) => View("InvalidSummoner", message);
    }
}
