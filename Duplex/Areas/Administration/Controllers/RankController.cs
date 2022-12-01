using Duplex.Core.Common.Constants;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models;
using Duplex.Core.Models.Administration.Rank;
using Duplex.Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Xml.Linq;

namespace Duplex.Areas.Administration.Controllers
{
    [Area(AreaConstants.AdministrationArea)]
    public class RankController : Controller
    {
        #region Injection
        private readonly IRankService rankService;
        private readonly RoleManager<IdentityRole> roleManager;
        public RankController(IRankService _rankService, RoleManager<IdentityRole> _roleManager)
        {
            rankService = _rankService;
            roleManager = _roleManager;
        }
        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(AddRankModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await rankService.CreateRankAsync(model.Name);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(All));
                }
                else
                    ModelState.AddModelError("", "Something went wrong!");
            }
            return View(model);
        }
        #endregion

        #region All

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var model = await rankService.GetAllAsync();
            return View(model);
        }

        #endregion

        #region Delete

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await rankService.DeleteRankAsync(id);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region Edit

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var response = await rankService.GetRankAsync(id);

            if (response == null)
            {
                throw new ArgumentException("Model is null!");
            }

            var model = new RankModel()
            {
                Id = id,
                Name = response.Name
            };

            TempData["rid"] = model.Id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, EditRankModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            if (TempData["rid"]?.ToString() != id)
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            model.Id = id;
            await rankService.EditRankAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
