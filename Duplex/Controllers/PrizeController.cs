using Duplex.Core.Contracts;
using Duplex.Core.Models.Event;
using Duplex.Core.Models.Prize;
using Duplex.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    public class PrizeController : Controller
    {
        #region Injection
        private readonly IPrizeService prizeService;
        public PrizeController(IPrizeService _prizeService)
        {
            this.prizeService = _prizeService;
        }
        #endregion

        #region Add

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add() => View();

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(AddPrizeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await prizeService.AddPrizeAsync(model);

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
                var model = await prizeService.GetAllAsync();
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
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await prizeService.DeletePrizeAsync(id);
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
            if (!await prizeService.Exists(id))
            {
                return RedirectToAction(nameof(All));
            }

            var response = await prizeService.GetPrizeAsync(id);

            var model = new EditPrizeModel()
            {
                Id = id,
                Name = response.Name,
                Cost = response.Cost,
                Description = response.Description,
                ImageUrl = response.ImageUrl
            };

            TempData["pid"] = model.Id;
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, EditPrizeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            if (TempData["pid"]?.ToString() != id.ToString())
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            model.Id = id;
            await prizeService.EditPrizeAsync(model);
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
                var model = await prizeService.GetPrizeAsync(id);
                return View(model);
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion
    }
}
