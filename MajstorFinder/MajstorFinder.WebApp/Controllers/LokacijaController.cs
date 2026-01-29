using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.Models;
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class LokacijaController : AdminController
    {
        private readonly ILokacijaService _lokacije;

        public LokacijaController(ILokacijaService lokacije)
        {
            _lokacije = lokacije;
        }

        // LIST + SEARCH + PAGING
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 5)
        {
            var list = await _lokacije.GetAllAsync(q, page, pageSize);
            var total = await _lokacije.CountAsync(q);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Q = q ?? "";

            // map u VM (da view ne koristi DAL direktno)
            var vm = list.Select(l => new LokacijaVm
            {
                Id = l.Id,
                Name = l.Name
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new LokacijaVm());

        [HttpPost]
        public async Task<IActionResult> Create(LokacijaVm model)
        {
            if (!ModelState.IsValid) return View(model);

            await _lokacije.CreateAsync(new Lokacija
            {
                Name = model.Name
            });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var l = await _lokacije.GetByIdAsync(id);
            if (l == null) return NotFound();

            return View(new LokacijaVm
            {
                Id = l.Id,
                Name = l.Name
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LokacijaVm model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var ok = await _lokacije.UpdateAsync(id, new Lokacija
            {
                Id = model.Id,
                Name = model.Name
            });

            if (!ok)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _lokacije.DeleteAsync(id);
            if (!ok) TempData["Err"] = "Ne mogu obrisati lokaciju (možda je vezana uz tvrtke).";

            return RedirectToAction(nameof(Index));
        }

        // AJAX DELETE
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var ok = await _lokacije.DeleteAsync(id);
            if (!ok) return BadRequest("Ne mogu obrisati lokaciju.");

            return Ok();
        }
    }
}