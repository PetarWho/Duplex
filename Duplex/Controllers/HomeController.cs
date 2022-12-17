using Duplex.Core.Contracts;
using Duplex.Core.Models.Index;
using Duplex.Infrastructure.Data.Models.Account;
using Duplex.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Duplex.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEventService eventService;
        private readonly UserManager<ApplicationUser> userManager;

        public HomeController(IEventService _eventService, UserManager<ApplicationUser> _userManager)
        {
            eventService = _eventService;
            userManager = _userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                if (!user.DailyClaimedOnUtc.HasValue || user.DailyClaimedOnUtc.Value.AddDays(1) <= DateTime.UtcNow)
                {
                    user.DailyAvailable = true;
                }
            }

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