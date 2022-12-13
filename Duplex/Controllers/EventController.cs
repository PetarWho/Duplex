using Duplex.Core.Common;
using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Data;
using Duplex.Infrastructure.Data.Models;
using Duplex.Infrastructure.Data.Models.Account;
using Google.Apis.Drive.v3.Data;
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
        private readonly ICategoryService categoryService;
        private readonly IRepository repo;
        private readonly IRiotService riotService;
        public EventController(IEventService _eventService, IRepository _repo,
            IRiotService _riot, ApplicationDbContext _context, ICategoryService _categoryService)
        {
            eventService = _eventService;
            repo = _repo;
            riotService = _riot;
            categoryService = _categoryService;
        }
        #endregion

        #region Add

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AddEventModel()
            {
                Categories = await categoryService.GetAllAsync()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddEventModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Categories = await categoryService.GetAllAsync();
                return View(model);
            }
            try
            {
                await eventService.AddEventAsync(model);

                return RedirectToAction(nameof(All));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Oops.. That was not supposed to happen");

                model.Categories = await categoryService.GetAllAsync();
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

                var model = await eventService.GetEventWithParticipantsAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
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
                var ev = await eventService.GetEventWithParticipantsAsync(id, User.FindFirstValue(ClaimTypes.NameIdentifier));
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

                return RedirectToAction("Joined", "Event", new { userId });
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

                return RedirectToAction("Joined", "Event", new { userId });
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
                    Description = x.Event.Description,
                    EntryCost = x.Event.EntryCost,
                    CreatedOnUTC = x.Event.CreatedOnUTC,
                    UserName = user.UserName,
                    Participants = x.Event.Participants
                }).ToListAsync();

            return View(model);
        }

        #endregion

        #region Verify

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(Guid id, string? matchId)
        {
            try
            {
                if (matchId == null || matchId.Trim() == "")
                {
                    return RedirectToAction("InvalidSummoner", "Error", new { message = "Invalid Match", area = "Errors" });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var eventUser = await repo.AllReadonly<EventUser>(x => x.UserId == userId && x.EventId == id).Include(x => x.Event.Category).ToListAsync();

                if (eventUser.Count != 0)
                {
                    var user = await repo.AllReadonly<ApplicationUser>().Include(x => x.Region).FirstOrDefaultAsync(x => x.Id == userId);
                    var ev = await repo.AllReadonly<Event>().Include(x => x.Category).FirstOrDefaultAsync(x => x.Id == id);

                    if (user == null || ev == null)
                    {
                        return RedirectToAction("_404", "Error", new { area = "Errors" });
                    }

                    var region = user?.Region.Code switch
                    {
                        "NA" => "NA1",
                        "EUNE" => "EUN1",
                        _ => "Unset",
                    };

                    var server = user?.Region.Code switch
                    {
                        "NA" => "americas",
                        "EUNE" => "europe",
                        _ => "Unset",
                    };

                    // Challenge 
                    string[] split = ev.Name.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                    int objectiveValue = 0;
                    string objective = ev.Category.Name.ToLower();

                    if (split.Contains("a") || split.Contains("an"))
                    {
                        objectiveValue = 1;
                    }
                    else
                    {
                        foreach (var word in split)
                        {
                            if (int.TryParse(word, out int result))
                            {
                                objectiveValue = result;
                                break;
                            }
                        }
                    }

                    var game = await riotService.GetMatchByMatchIdAsync(matchId, server, region);
                    if (game == null)
                    {
                        return RedirectToAction("InvalidSummoner", "Error", new { message = "Invalid Match", area = "Errors" });
                    }

                    var participant = game?.info.participants.FirstOrDefault(x => x.puuid == user?.PUUID);

                    object? target = null;

                    target = objective switch
                    {
                        "kill" => participant?.kills,
                        "minions" => participant?.totalMinionsKilled,
                        "pentakill" => participant?.pentaKills,
                        "win" => participant?.win,
                        _ => throw new ArgumentException("Category Not Implemented!"),
                    };
                    if (target == null)
                    {
                        return RedirectToAction("InvalidSummoner", "Error", new { message = "Invalid Match", area = "Errors" });
                    }

                    if (target.GetType() == typeof(int))
                    {
                        if ((int)target >= objectiveValue)
                        {
                            await eventService.VerifyDone(id, userId);
                        }
                        else
                        {
                            return RedirectToAction("InvalidSummoner", "Error", new { message = "Challenge not completed", area = "Errors" });
                        }
                    }
                    else if (target.GetType() == typeof(bool))
                    {
                        if ((bool)target)
                        {
                            await eventService.VerifyDone(id, userId);
                        }
                        else
                        {
                            return RedirectToAction("InvalidSummoner", "Error", new { message = "Challenge not completed", area = "Errors" });
                        }
                    }
                    else
                    {
                        return RedirectToAction("InvalidSummoner", "Error", new { message = "Invalid Match Data", area = "Errors" });
                    }
                }

                return RedirectToAction("Joined", "Event", new { userId });
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion
    }
}
