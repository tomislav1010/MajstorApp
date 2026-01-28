using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services; 
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class VrstaRadaController : Controller
    {
        private readonly IVrstaRadaService _vrste;
        private readonly ITvrtkaService _tvrtke;

        public VrstaRadaController(IVrstaRadaService vrste, ITvrtkaService tvrtke)
        {
            _vrste = vrste;
            _tvrtke = tvrtke;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("isAdmin") == "1";

        // LIST + FILTER + PAGING
        public async Task<IActionResult> Index(int? tvrtkaId, int page = 1, int pageSize = 5)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            // dropdown tvrtki
            var sveTvrtke = await _tvrtke.GetAllAsync(null, 1, 1000);
            ViewBag.Tvrtke = sveTvrtke
                .Select(t => new TvrtkaVm { Id = t.Id, Name = t.Name })
                .ToList();

            ViewBag.SelectedTvrtkaId = tvrtkaId;

            // paged data
            var vrste = await _vrste.GetAllAsync(tvrtkaId, page, pageSize);
            var total = await _vrste.CountAsync(tvrtkaId);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);

            var vm = vrste.Select(v => new VrstaRadaVm
            {
                Id = v.Id,
                Name = v.Name,
                TvrtkaId = v.TvrtkaId
            }).ToList();

            return View(vm);
        }

        // CREATE GET
        [HttpGet]
        public async Task<IActionResult> Create(int? tvrtkaId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var sveTvrtke = await _tvrtke.GetAllAsync(null, 1, 1000);
            ViewBag.Tvrtke = sveTvrtke
                .Select(t => new TvrtkaVm { Id = t.Id, Name = t.Name })
                .ToList();

            ViewBag.SelectedTvrtkaId = tvrtkaId;

            return View(new VrstaRadaVm { TvrtkaId = tvrtkaId ?? 0 });
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(VrstaRadaVm model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            if (model.TvrtkaId <= 0)
                ModelState.AddModelError(nameof(model.TvrtkaId), "Tvrtka je obavezna.");

            if (!ModelState.IsValid)
            {
                var sveTvrtke = await _tvrtke.GetAllAsync(null, 1, 1000);
                ViewBag.Tvrtke = sveTvrtke.Select(t => new TvrtkaVm { Id = t.Id, Name = t.Name }).ToList();
                ViewBag.SelectedTvrtkaId = model.TvrtkaId;
                return View(model);
            }

            await _vrste.CreateAsync(new MajstorFinder.DAL.Models.VrstaRada
            {
                Name = model.Name,
                TvrtkaId = model.TvrtkaId
            });

            return RedirectToAction(nameof(Index), new { tvrtkaId = model.TvrtkaId });
        }

        // EDIT GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var vrsta = await _vrste.GetByIdAsync(id);
            if (vrsta == null) return NotFound();

            var sveTvrtke = await _tvrtke.GetAllAsync(null, 1, 1000);
            ViewBag.Tvrtke = sveTvrtke.Select(t => new TvrtkaVm { Id = t.Id, Name = t.Name }).ToList();
            ViewBag.SelectedTvrtkaId = vrsta.TvrtkaId;

            var vm = new VrstaRadaVm
            {
                Id = vrsta.Id,
                Name = vrsta.Name,
                TvrtkaId = vrsta.TvrtkaId
            };

            return View(vm);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, VrstaRadaVm model)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");
            if (id != model.Id) return BadRequest();

            if (model.TvrtkaId <= 0)
                ModelState.AddModelError(nameof(model.TvrtkaId), "Tvrtka je obavezna.");

            if (!ModelState.IsValid)
            {
                var sveTvrtke = await _tvrtke.GetAllAsync(null, 1, 1000);
                ViewBag.Tvrtke = sveTvrtke.Select(t => new TvrtkaVm { Id = t.Id, Name = t.Name }).ToList();
                ViewBag.SelectedTvrtkaId = model.TvrtkaId;
                return View(model);
            }

            var ok = await _vrste.UpdateAsync(id, new MajstorFinder.DAL.Models.VrstaRada
            {
                Id = model.Id,
                Name = model.Name,
                TvrtkaId = model.TvrtkaId
            });

            if (!ok)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju.");
                return View(model);
            }

            return RedirectToAction(nameof(Index), new { tvrtkaId = model.TvrtkaId });
        }

        // DELETE (POST)
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int? tvrtkaId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Auth");

            var ok = await _vrste.DeleteAsync(id);
            if (!ok) TempData["Err"] = "Ne mogu obrisati (možda je vezano uz zahtjeve).";

            return RedirectToAction(nameof(Index), new { tvrtkaId });
        }

        // DELETE AJAX (DELETE)
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            if (!IsAdmin()) return Forbid();

            var ok = await _vrste.DeleteAsync(id);
            if (!ok) return BadRequest("Brisanje nije uspjelo.");

            return Ok();
        }
    }
}

