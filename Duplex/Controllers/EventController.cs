using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    public class EventController : Controller
    {
        #region Injection
        private readonly IEventService eventService;
        public EventController(IEventService _eventService)
        {
            this.eventService = _eventService;
        }
        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add() => View();

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
            return View(model);
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            await eventService.DeleteEventAsync(id);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {

            if ((await eventService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            var response = await eventService.GetEventAsync(id);

            TempData["eid"] = id;

            var model = new EditEventModel()
            {
                Id = response.Id,
                Name = response.Name,
                EntryCost = response.EntryCost,
                Description = response.Description,
                ImageUrl = response.ImageUrl
            };
            return View(model);
        }

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
    }
}
