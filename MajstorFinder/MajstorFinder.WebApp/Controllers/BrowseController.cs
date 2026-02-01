using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.Models;
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ITvrtkaService _tvrtke;
        private readonly IVrstaRadaService _vrste;
        private readonly ILokacijaService _lokacije;
        private readonly ITvrtkaLokacijaService _tvrtkaLokacije;

        public BrowseController(
            ITvrtkaService tvrtke,
            IVrstaRadaService vrste,
            ILokacijaService lokacije,
            ITvrtkaLokacijaService tvrtkaLokacije)
        {
            _tvrtke = tvrtke;
            _vrste = vrste;
            _lokacije = lokacije;
            _tvrtkaLokacije = tvrtkaLokacije;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
    int? lokacijaId,
    int? vrstaRadaId,
    int page = 1,
    int pageSize = 6)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 6;

            // dropdown podaci (uzimaš sve)
            var lokacije = await _lokacije.GetAllAsync(q: null, page: 1, pageSize: 1000);
            var vrste = await _vrste.GetAllAsync(tvrtkaId: null, page: 1, pageSize: 1000);

            // tvrtke (filtriramo pa paginiramo)
            var tvrtke = await _tvrtke.GetAllAsync(q: null, page: 1, pageSize: 1000);

            // FILTER: vrsta rada
            if (vrstaRadaId.HasValue)
            {
                var vrsta = vrste.FirstOrDefault(v => v.Id == vrstaRadaId.Value);
                tvrtke = (vrsta == null)
                    ? new List<MajstorFinder.DAL.Models.Tvrtka>()
                    : tvrtke.Where(t => t.Id == vrsta.TvrtkaId).ToList();
            }

            // FILTER: lokacija (tvrtke koje rade na toj lokaciji)
            if (lokacijaId.HasValue)
            {
                // OVO treba vraćati TVRTKA ID-eve za lokaciju
                var tvrtkaIds = await _tvrtkaLokacije.GetLokacijeIdsForTvrtkaAsync(lokacijaId.Value);
                tvrtke = tvrtke.Where(t => tvrtkaIds.Contains(t.Id)).ToList();
            }

            // PAGING
            int totalItems = tvrtke.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var pageItems = tvrtke
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new BrowseVm
            {
                LokacijaId = lokacijaId,
                VrstaRadaId = vrstaRadaId,
                Lokacije = lokacije.Select(ToLokacijaVm).ToList(),
                VrsteRada = vrste.Select(ToVrstaRadaVm).ToList(),
                Tvrtke = pageItems.Select(ToTvrtkaVm).ToList(),

                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var tvrtka = await _tvrtke.GetByIdAsync(id);
            if (tvrtka == null) return NotFound();

            var vrste = await _vrste.GetByTvrtkaAsync(id);
            var lokacije = await _tvrtkaLokacije.GetLokacijeForTvrtkaAsync(id);

            var vm = new TvrtkaVm
            {
                Id = tvrtka.Id,
                Name = tvrtka.Name,
                Description = tvrtka.Description,
                Phone = tvrtka.Phone,
                Email = tvrtka.Email
            };

            ViewBag.Vrste = vrste.Select(ToVrstaRadaVm).ToList();
            ViewBag.Lokacije = lokacije.Select(ToLokacijaVm).ToList();

            return View(vm);
        }

        // ===== map metode =====
        private static TvrtkaVm ToTvrtkaVm(MajstorFinder.DAL.Models.Tvrtka t) => new()
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            Phone = t.Phone,
            Email = t.Email
        };

        private static VrstaRadaVm ToVrstaRadaVm(MajstorFinder.DAL.Models.VrstaRada v) => new()
        {
            Id = v.Id,
            Name = v.Name,
            TvrtkaId = v.TvrtkaId
        };

        private static LokacijaVm ToLokacijaVm(MajstorFinder.DAL.Models.Lokacija l) => new()
        {
            Id = l.Id,
            Name = l.Name
        };
    }
}