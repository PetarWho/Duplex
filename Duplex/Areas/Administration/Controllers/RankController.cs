using Duplex.Core.Contracts.Administration;
using Duplex.Core.Models.Administration.Rank;
using Microsoft.AspNetCore.Mvc;

namespace Duplex.Areas.Administration.Controllers
{
    public class RankController : Controller
    {
        #region Injection
        private readonly IRankService rankService;
        public RankController(IRankService _rankService)
        {
            this.rankService = _rankService;
        }
        #endregion

        #region Add

        [HttpGet]
        public IActionResult Add() => View();

        [HttpPost]
        public async Task<IActionResult> Add(AddRankModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await rankService.AddEventAsync(model);

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
            var model = await rankService.GetAllAsync();
            return View(model);
        }

        #endregion
    }
}
