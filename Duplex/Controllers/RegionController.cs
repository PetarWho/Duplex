using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Duplex.Core.Services;
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
            try
            {
                var model = await regionService.GetAllAsync();
            return View(model.OrderBy(x=>x.Name));
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
            await regionService.DeleteRegionAsync(id);
            return RedirectToAction(nameof(All));
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!await regionService.Exists(id))
            {
                return RedirectToAction(nameof(All));
            }

            var model = await regionService.GetRegionAsync(id);

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
