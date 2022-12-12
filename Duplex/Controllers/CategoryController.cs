using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Duplex.Core.Models.Category;
using Duplex.Infrastructure.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        #region Injection
        private readonly ICategoryService categoryService;
        public CategoryController(ICategoryService _categoryService)
        {
            this.categoryService = _categoryService;
        }
        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await categoryService.AddCategoryAsync(model);

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
                var model = await categoryService.GetAllAsync();
                return View(model.OrderBy(x => x.Name));
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await categoryService.DeleteCategoryAsync(id);
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
            if (!await categoryService.Exists(id))
            {
                return RedirectToAction(nameof(All));
            }

            var model = await categoryService.GetCategoryAsync(id);

            TempData["cid"] = model.Id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
            {
                return RedirectToAction("_403", "Error", new { area = "Errors" });
            }

            if (TempData["cid"]?.ToString() != id.ToString())
            {
                return RedirectToAction("_403", "Error", new { area = "Errors" });
            }

            model.Id = id;
            await categoryService.EditCategoryAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
