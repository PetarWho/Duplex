using Duplex.Core.Contracts;
using Duplex.Core.Models.Prize;
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

        [HttpGet]
        public IActionResult Add() => View();

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
            var model = await prizeService.GetAllAsync();
            return View(model);
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete(Guid pId)
        {
            await prizeService.DeletePrizeAsync(pId);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpPost]
        public async Task<IActionResult> Edit(Guid pId)
        {
            var response = await prizeService.GetPrizeAsync(pId);

            if (response == null)
            {
                throw new ArgumentException("Model is null!");
            }

            var model = new EditPrizeModel()
            {
                Id = response.Id,
                Name = response.Name,
                Cost = response.Cost,
                Description = response.Description,
                ImageUrl = response.ImageUrl
            };

            TempData["pid"] = model.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitEdit(EditPrizeModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var id = TempData?["pid"]?.ToString();

            if (id == null)
            {
                throw new ArgumentException("Non-existing element");
            }

            model.Id = Guid.Parse(id);
            await prizeService.EditPrizeAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
