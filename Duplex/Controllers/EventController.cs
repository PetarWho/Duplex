using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Duplex.Controllers
{
    public class EventController : Controller
    {
        #region Injection
        private readonly IEventService eventService;
        private readonly IRepository repo;
        public EventController(IEventService _eventService, IRepository _repo)
        {
            eventService = _eventService;
            repo = _repo;
        }
        #endregion

        #region Add

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AddEventModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await eventService.AddEventAsync(model);

                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Oops.. That was not supposed to happen");

                return View(model);
            }
        }
        #endregion

        #region All

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await eventService.GetAllAsync();
            return View(model.OrderByDescending(x=>x.CreatedOnUTC));
        }

        #endregion

        #region Delete

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            if ((await eventService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            var model = await eventService.GetEventAsync(id);

            TempData["eid"] = id;

            
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditEventModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            if (TempData["eid"]?.ToString() != id.ToString())
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            model.Id = id;
            await eventService.EditEventAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Details

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            if(id == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }

            var ev = await eventService.GetEventWithParticipantsAsync(id);

            var model = new DetailsEventModel()
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                TeamSize = ev.TeamSize,
                EntryCost = ev.EntryCost,
                ImageUrl = ev.ImageUrl,
                CreatedOnUTC = ev.CreatedOnUTC,
                Participants = ev.Participants
            };

            return View(model);
        }

        #endregion

        #region Join

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Join(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var match = await repo.AllReadonly<EventUser>(x => x.UserId == userId && x.EventId == id).ToListAsync();

            if (match.Count==0){
                await eventService.JoinEvent(id, userId);
            }

            return RedirectToAction("Profile", "Account");
        }

        #endregion

        #region Leave

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Leave(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var match = await repo.AllReadonly<EventUser>(x => x.UserId == userId && x.EventId == id).ToListAsync();

            if (match.Count != 0)
            {
                await eventService.LeaveEvent(id, userId);
            }

            return RedirectToAction("Profile", "Account");
        }

        #endregion

        #region Joined

        [HttpGet]
        public async Task<IActionResult> Joined(string userId)
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);

            var model = await repo.AllReadonly<EventUser>()
                .Include(x => x.Event).Where(x=>x.UserId==userId).Select(x=> new EventModel()
            {
                    Id = x.Event.Id,
                    Name=x.Event.Name,
                    ImageUrl = x.Event.ImageUrl,
                    TeamSize=x.Event.TeamSize,
                    Description = x.Event.Description,
                    EntryCost = x.Event.EntryCost,
                    CreatedOnUTC = x.Event.CreatedOnUTC
            }).ToListAsync();

            return View(model);
        }

        #endregion
    }
}
