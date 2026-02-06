using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.BLL.Services; 
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

        
        [HttpGet]
        public async Task<IActionResult> Moji()
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            var list = await _zahtjevi.GetByKorisnikAsync(CurrentUserId);

            var tvrtke = (await _tvrtke.GetAllAsync(null, 1, 1000))
                .ToDictionary(x => x.Id, x => x.Name);

            var vrste = (await _vrste.GetAllAsync(null, 1, 1000))
                .ToDictionary(x => x.Id, x => x.Name);

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

 
        [HttpGet]
        public async Task<IActionResult> Create(int tvrtkaId, int? vrstaRadaId)
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            if (tvrtkaId <= 0) return BadRequest("TvrtkaId je obavezan.");

            ViewBag.TvrtkaId = tvrtkaId;
            ViewBag.Vrste = await _vrste.GetByTvrtkaAsync(tvrtkaId);

            return View(new CreateZahtjevDto
            {
                TvrtkaId = tvrtkaId,
                VrstaRadaId = vrstaRadaId ?? 0
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateZahtjevDto dto)
        {
            if (CurrentUserId == 0) return RedirectToAction("Login", "Auth");

            // ako nemaš Required na DTO, ovo osigura minimum validacije
            if (dto.TvrtkaId <= 0) ModelState.AddModelError("", "Tvrtka je obavezna.");
            if (dto.VrstaRadaId <= 0) ModelState.AddModelError("", "Vrsta rada je obavezna.");
            if (string.IsNullOrWhiteSpace(dto.Description)) ModelState.AddModelError(nameof(dto.Description), "Opis je obavezan.");

            if (!ModelState.IsValid)
            {
                ViewBag.TvrtkaId = dto.TvrtkaId;
                ViewBag.Vrste = await _vrste.GetByTvrtkaAsync(dto.TvrtkaId);
                return View(dto);
            }

            await _zahtjevi.CreateAsync(CurrentUserId, dto);
            return RedirectToAction(nameof(Moji));
        }


        [HttpGet]
        public async Task<IActionResult> AdminIndex()
        {
            if (!IsAdmin) return Forbid();

            var list = await _zahtjevi.GetAllAsync();

            var tvrtke = (await _tvrtke.GetAllAsync(null, 1, 1000))
                .ToDictionary(x => x.Id, x => x.Name);

            var vrste = (await _vrste.GetAllAsync(null, 1, 1000))
                .ToDictionary(x => x.Id, x => x.Name);

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

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(AdminIndex));


        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateZahtjevStatusDto dto)
        {
            if (!IsAdmin) return Forbid();
            if (dto == null || string.IsNullOrWhiteSpace(dto.Status))
                return BadRequest("Status je obavezan.");

            await _zahtjevi.UpdateStatusAsync(id, dto.Status);
            return Ok();
        }

        // =========================
        // ADMIN: delete
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin) return Forbid();

            await _zahtjevi.DeleteAsync(id);
            return RedirectToAction(nameof(AdminIndex));
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            if (!IsAdmin) return Forbid();

            if (string.IsNullOrWhiteSpace(status))
            {
                TempData["Err"] = "Status je obavezan.";
                return RedirectToAction(nameof(AdminIndex));
            }

            await _zahtjevi.UpdateStatusAsync(id, status);
            return RedirectToAction(nameof(AdminIndex));
        }

        public class UpdateZahtjevStatusDto
        {
            public string Status { get; set; } = "";
        }
    }
}