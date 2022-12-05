using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RegionController : Controller
    {
        #region Injection
        private readonly IRegionService regionService;
        public RegionController(IRegionService _regionService)
        {
            this.regionService = _regionService;
        }
        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(RegionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await regionService.AddRegionAsync(model);

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
            var model = await regionService.GetAllAsync();
            return View(model);
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            await regionService.DeleteRegionAsync(id);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await regionService.GetRegionAsync(id);

            if (response == null)
            {
                throw new ArgumentException("Model is null!");
            }

            var model = new RegionModel()
            {
                Id = id,
                Name = response.Name,
                Code = response.Code
            };

            TempData["rid"] = model.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, RegionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
            {
                return RedirectToAction("_403", "Error", new { area = "Errors" });
            }

            if (TempData["rid"]?.ToString() != id.ToString())
            {
                return RedirectToAction("_403", "Error", new { area = "Errors" });
            }

            model.Id = id;
            await regionService.EditRegionAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
