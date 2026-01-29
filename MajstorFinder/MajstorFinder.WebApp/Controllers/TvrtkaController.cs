using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.Models;


namespace MajstorFinder.WebApp.Controllers
{
    // AdminController ti i dalje štiti admin dio (session/role)
    public class TvrtkaController : AdminController
    {
        private readonly ITvrtkaService _tvrtkaService;
        private readonly ILokacijaService _lokacijaService;
        private readonly ITvrtkaLokacijaService _tvrtkaLokacije;


        public TvrtkaController(ITvrtkaService tvrtkaService, ILokacijaService lokacijaService, ITvrtkaLokacijaService tvrtkaLokacije)
        {
            _tvrtkaService = tvrtkaService;
            _lokacijaService = lokacijaService;
            _tvrtkaLokacije = tvrtkaLokacije;
        }

        // LIST + search + paging (sad je "pravo" paging u BLL-u, ne klijentski)
        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 5)
        {
            var items = await _tvrtkaService.GetAllAsync(q, page, pageSize);
            var total = await _tvrtkaService.CountAsync(q);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Q = q ?? "";

            // map Entity -> Vm (da view ostane isti)
            var vm = items.Select(t => new TvrtkaVm
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Phone = t.Phone,
                Email = t.Email
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View(new TvrtkaVm());

        [HttpPost]
        public async Task<IActionResult> Create(TvrtkaVm model)
        {
            if (!ModelState.IsValid) return View(model);

            // BLL zapisuje u DB preko DAL-a
            await _tvrtkaService.CreateAsync(new Tvrtka
            {
                Name = model.Name,
                Description = model.Description,
                Phone = model.Phone,
                Email = model.Email
            });

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var t = await _tvrtkaService.GetByIdAsync(id);
            if (t == null) return NotFound();

            var vm = new TvrtkaVm
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Phone = t.Phone,
                Email = t.Email
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TvrtkaVm model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var ok = await _tvrtkaService.UpdateAsync(id, new Tvrtka
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Phone = model.Phone,
                Email = model.Email
            });

            if (!ok) return NotFound();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Ako ne uspije (npr. FK constraint), service vrati false ili baci exception
            var ok = await _tvrtkaService.DeleteAsync(id);

            if (!ok)
                TempData["Err"] = "Ne mogu obrisati tvrtku (možda je vezana uz vrste rada/zahtjeve).";

            return RedirectToAction(nameof(Index));
        }

        // AJAX delete (ako ga koristiš u viewu)
        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var ok = await _tvrtkaService.DeleteAsync(id);
            if (!ok) return BadRequest("Ne mogu obrisati tvrtku (možda je vezana uz vrste rada/zahtjeve).");
            return Ok();
        }

        // LOKACIJE view: prikaz svih lokacija + trenutno vezanih lokacija za tvrtku
        [HttpGet]
        public async Task<IActionResult> Lokacije(int id)
        {
            var tvrtka = await _tvrtkaService.GetByIdAsync(id);
            if (tvrtka == null) return NotFound();

            var sveLokacije = await _lokacijaService.GetAllAsync(null, 1, int.MaxValue);
            var postojece = await _tvrtkaService.GetLokacijeAsync(id); // ovo moraš imati u ITvrtkaService + TvrtkaService

            var set = postojece.Select(x => x.Id).ToHashSet();

            var vm = new TvrtkaLokacijeVm
            {
                TvrtkaId = tvrtka.Id,
                TvrtkaName = tvrtka.Name,
                Lokacije = sveLokacije.Select(l => new LokacijaCheckVm
                {
                    Id = l.Id,
                    Name = l.Name,
                    Selected = set.Contains(l.Id)
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Lokacije(TvrtkaLokacijeVm model)
        {
            var selectedIds = model.Lokacije
                .Where(x => x.Selected)
                .Select(x => x.Id)
                .ToList();

            await _tvrtkaLokacije.SetLokacijeForTvrtkaAsync(model.TvrtkaId, selectedIds);

            return RedirectToAction(nameof(Index));
        }
    }
}
