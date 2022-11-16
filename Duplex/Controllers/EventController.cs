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

        [HttpPost]
        public async Task<IActionResult> Delete(Guid eId)
        {
            await eventService.DeleteEventAsync(eId);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpPost]
        public async Task<IActionResult> Edit(Guid eId)
        {
            var response = await eventService.GetEventAsync(eId);

            if (response == null)
            {
                throw new ArgumentException("Model is null!");
            }

            var model = new EditEventModel()
            {
                Id = response.Id,
                Name = response.Name,
                EntryCost = response.EntryCost,
                Description = response.Description,
                ImageUrl = response.ImageUrl
            };

            TempData["eid"] = model.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEdit(EditEventModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = TempData?["eid"]?.ToString();

            if (id == null)
            {
                throw new ArgumentException("Non-existing element");
            }

            model.Id = Guid.Parse(id);
            await eventService.EditEventAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
