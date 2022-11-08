using Microsoft.AspNetCore.Mvc;
using Duplex.Core.Contracts;
using Duplex.Core.Models;
using Duplex.Core.Services;

namespace Duplex.Controllers
{
    public class RegionController : Controller
    {
       private readonly IRegionService regionService;
        public RegionController(IRegionService _regionService)
        {
            this.regionService = _regionService;
        }

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

                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Oops.. That was not supposed to happen");

                return View(model);
            }
        }
    }
}
