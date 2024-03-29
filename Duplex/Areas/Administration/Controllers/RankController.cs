﻿using Duplex.Core.Common;
using Duplex.Core.Common.Constants;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Administration.Controllers
{
    [Area(AreaConstants.AdministrationArea)]
    [Authorize(Roles = "Admin")]
    public class RankController : Controller
    {
        #region Injection
        private readonly IRankService rankService;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRepository repo;
        private readonly UserManager<ApplicationUser> userManager;
        public RankController(IRankService _rankService, RoleManager<IdentityRole> _roleManager, IRepository _repo, UserManager<ApplicationUser> _userManager)
        {
            rankService = _rankService;
            roleManager = _roleManager;
            repo = _repo;
            userManager = _userManager;
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
            try
            {
                var model = await rankService.GetAllAsync();
                return View(model.Select(r => new RankModel()
                {
                    Id = r.Id,
                    Name = r.Name
                }).ToList());
            }
            catch (Exception)
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await rankService.DeleteRankAsync(id);
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
        public async Task<IActionResult> Edit(string id)
        {
            if(!await rankService.Exists(id))
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }

            var response = await rankService.GetRankAsync(id);

            var model = new RankModel()
            {
                Id = id,
                Name = response.Name
            };

            TempData["raid"] = model.Id;
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

            if (TempData["raid"]?.ToString() != id)
            {
                return RedirectToPage("/Error/_403", new { area = "Errors" });
            }

            model.Id = id;
            await rankService.EditRankAsync(model);
            return RedirectToAction(nameof(All));
        }

        #endregion

        #region

        [HttpGet]
        public async Task<IActionResult> Set()
        {
            var roles = await rankService.GetAllAsync();
            var model = new SetRankModel()
            {
                Ranks = roles
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Set(SetRankModel model)
        {
            if(!await rankService.Exists(model.RankId))
            {
                return RedirectToAction("_502", "Error", new { area = "Errors" });
            }

            if (!ModelState.IsValid)
            {
                model.Ranks = await rankService.GetAllAsync(); ;
                return View(model);
            }

            var user = await repo.GetByIdAsync<ApplicationUser>(model.UserId);
            var rank = await roleManager.FindByIdAsync(model.RankId);

            if (user == null || rank == null)
            {
                throw new Exception("Incorrect user or rank!");
            }

            await userManager.AddToRoleAsync(user, rank.Name);

            return RedirectToAction(nameof(All));
        }

        #endregion
    }
}
