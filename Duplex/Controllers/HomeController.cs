using Duplex.Core.Contracts;
using Duplex.Core.Models.Index;
using Duplex.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Duplex.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService eventService;

        public HomeController(IEventService _eventService)
        {
            eventService = _eventService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new LastThreeEventsModel()
            {
                Events = await eventService.GetLastThree()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}