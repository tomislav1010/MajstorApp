using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services; // ili .Services (ovisno gdje ti je interface)
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
        [HttpGet]
        public async Task<IActionResult> Create(int tvrtkaId)
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            ViewBag.TvrtkaId = tvrtkaId;
            ViewBag.Vrste = await _vrste.GetByTvrtkaAsync(tvrtkaId);

            return View(new CreateZahtjevDto { TvrtkaId = tvrtkaId });
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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin) return Forbid();

            var list = await _zahtjevi.GetAllAsync();
            return View(list);
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