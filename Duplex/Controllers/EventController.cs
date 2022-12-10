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
        [ValidateAntiForgeryToken]
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
            try
            {
                var model = await eventService.GetAllAsync();
                return View(model.OrderByDescending(x => x.CreatedOnUTC));
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }

        }

        #endregion

        #region Delete

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await eventService.DeleteEventAsync(id);
                return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Edit

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            if (!await eventService.Exists(id))
            {
                return RedirectToAction(nameof(All));
            }

            var model = await eventService.GetEventAsync(id);

            TempData["eid"] = id;


            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            if (id == Guid.Empty)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                var model = await eventService.GetEventWithParticipantsAsync(id);
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Join

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(Guid id)
        {
            try
            {
                var ev = await eventService.GetEventWithParticipantsAsync(id);
                if(ev.Participants.Count() == ev.TeamSize * 2)
                {
                    return RedirectToAction(nameof(All));
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var match = await repo.AllReadonly<EventUser>(x => x.UserId == userId && x.EventId == id).ToListAsync();
                var user = await repo.GetByIdAsync<ApplicationUser>(userId);

                if (match.Count == 0)
                {
                    if (user.Coins >= ev.EntryCost)
                    {
                        await eventService.JoinEvent(id, userId);
                    }
                    else
                    {
                        return RedirectToAction("_502", "Error", new { area = "Errors" });
                    }
                }

                return RedirectToAction("Joined", "Event", new { userId = userId });
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Leave

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Leave(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var match = await repo.AllReadonly<EventUser>(x => x.UserId == userId && x.EventId == id).ToListAsync();

                if (match.Count != 0)
                {
                    await eventService.LeaveEvent(id, userId);
                }

                return RedirectToAction("Joined", "Event", new { userId = userId });
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Joined

        [HttpGet]
        public async Task<IActionResult> Joined(string userId)
        {
            var user = await repo.GetByIdAsync<ApplicationUser>(userId);

            if (user == null)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }

            var model = await repo.AllReadonly<EventUser>()
                .Include(x => x.Event).Where(x => x.UserId == userId).Select(x => new JoinedEventModel()
                {
                    EventId = x.Event.Id,
                    EventName = x.Event.Name,
                    EventImageUrl = x.Event.ImageUrl,
                    TeamSize = x.Event.TeamSize,
                    Description = x.Event.Description,
                    EntryCost = x.Event.EntryCost,
                    CreatedOnUTC = x.Event.CreatedOnUTC,
                    UserName = user.UserName,
                    Participants = x.Event.Participants
                }).ToListAsync();

            return View(model);
        }

        #endregion
    }
}
