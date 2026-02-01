using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services; // ili .Services (ovisno gdje ti je interface)
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly IZahtjevService _zahtjevi;
        private readonly ITvrtkaService _tvrtke;
        private readonly IVrstaRadaService _vrste;

        public ZahtjevController(IZahtjevService zahtjevi, ITvrtkaService tvrtke, IVrstaRadaService vrste)
        {
            _zahtjevi = zahtjevi;
            _tvrtke = tvrtke;
            _vrste = vrste;
        }

        private int CurrentUserId => HttpContext.Session.GetInt32("userId") ?? 0;
        private bool IsAdmin => HttpContext.Session.GetString("isAdmin") == "1";

        // KLIJENT: moji zahtjevi
        [HttpGet]
        public async Task<IActionResult> Moj()
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            var list = await _zahtjevi.GetByKorisnikAsync(CurrentUserId);
            return View(list);
        }

        // KLIJENT: create forma
        public async Task<IActionResult> Moji()
        {
            int korisnikId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (korisnikId == 0) return RedirectToAction("Login", "Auth");

            var list = await _zahtjevi.GetByKorisnikAsync(korisnikId);

            var tvrtke = (await _tvrtke.GetAllAsync(null, 1, 100)).ToDictionary(x => x.Id, x => x.Name);
            var vrste = (await _vrste.GetAllAsync(null, 1, 100)).ToDictionary(x => x.Id, x => x.Name);

            var vm = list.Select(z => new ZahtjevVm
            {
                Id = z.Id,
                KorisnikId = z.KorisnikId,
                TvrtkaId = z.TvrtkaId,
                VrstaRadaId = z.VrstaRadaId,
                Description = z.Description,
                Status = z.Status,
                DateCreated = z.DateCreated,
                TvrtkaName = tvrtke.TryGetValue(z.TvrtkaId, out var tn) ? tn : "",
                VrstaRadaName = vrste.TryGetValue(z.VrstaRadaId, out var vn) ? vn : ""
            }).ToList();

            return View(vm);
        }

        // KLIJENT: create submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateZahtjevDto dto)
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.TvrtkaId = dto.TvrtkaId;
                ViewBag.Vrste = await _vrste.GetByTvrtkaAsync(dto.TvrtkaId);
                return View(dto);
            }

            await _zahtjevi.CreateAsync(CurrentUserId, dto);
            return RedirectToAction(nameof(Moj));
        }

        // ADMIN: svi zahtjevi
        public async Task<IActionResult> Index()
        {
            int korisnikId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (korisnikId == 0) return RedirectToAction("Login", "Auth");

            var list = await _zahtjevi.GetByKorisnikAsync(korisnikId);

            var tvrtke = (await _tvrtke.GetAllAsync(null, 1, 100)).ToDictionary(x => x.Id, x => x.Name);
            var vrste = (await _vrste.GetAllAsync(null, 1, 100)).ToDictionary(x => x.Id, x => x.Name);

            var vm = list.Select(z => new ZahtjevVm
            {
                Id = z.Id,
                KorisnikId = z.KorisnikId,
                TvrtkaId = z.TvrtkaId,
                VrstaRadaId = z.VrstaRadaId,
                Description = z.Description,
                Status = z.Status,
                DateCreated = z.DateCreated,
                TvrtkaName = tvrtke.TryGetValue(z.TvrtkaId, out var tn) ? tn : "",
                VrstaRadaName = vrste.TryGetValue(z.VrstaRadaId, out var vn) ? vn : ""
            }).ToList();

            return View(vm);
        }

        // ADMIN: promjena statusa
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, UpdateZahtjevStatusDto dto)
        {
            if (!IsAdmin) return Forbid();
            if (string.IsNullOrWhiteSpace(dto.Status)) return BadRequest("Status je obavezan.");

            await _zahtjevi.UpdateStatusAsync(id, dto.Status);
            return Ok();
        }

        // ADMIN: delete
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin) return Forbid();
            await _zahtjevi.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }

    public class UpdateZahtjevStatusDto
    {
        public string Status { get; set; } = "";
    }
}