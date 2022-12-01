using Duplex.Core.Common;
using Duplex.Core.Common.Constants;
using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models.Administration.Rank;
using Duplex.Infrastructure.Data.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Administration.Controllers
{
    [Area(AreaConstants.AdministrationArea)]
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
            var model = await rankService.GetAllAsync();
            return View(model.Select(r => new RankModel()
            {
                Id = r.Id,
                Name = r.Name,
                ConcurrencyStamp = r.ConcurrencyStamp
            }));
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
                throw new ArgumentException("No such rank!");
            }

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
            if (!ModelState.IsValid || !await rankService.Exists(model.RankId))
            {
                model.Ranks = await rankService.GetAllAsync(); ;
                return View(model);
            }

            var user = await repo.GetByIdAsync<ApplicationUser>(model.UserId);
            var rank = await roleManager.FindByIdAsync(model.UserId);

            if (user == null || rank == null)
            {
                throw new Exception("Incorrect user or rank!");
            }

            return View(RedirectToAction(nameof(All)));
        }

        #endregion
    }
}
