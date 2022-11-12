using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    [Authorize]
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

        [HttpPost]
        public async Task<IActionResult> Delete(int regId)
        {
            await regionService.DeleteRegionAsync(regId);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpPost]
        public async Task<IActionResult> Edit(int regId)
        {
            var response = await regionService.GetRegionAsync(regId);

            if (response == null)
            {
                throw new ArgumentException("Model is null!");
            }

            var model = new RegionModel()
            {
                Id = response.Id,
                Name = response.Name,
                Code = response.Code
            };

            TempData["clickedId"] = model.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEdit(RegionModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if(!int.TryParse(TempData?["clickedId"]?.ToString(), out int id))
            {
                throw new ArgumentException("Non-existing element");
            }

            model.Id = id;
            await regionService.EditRegionAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
