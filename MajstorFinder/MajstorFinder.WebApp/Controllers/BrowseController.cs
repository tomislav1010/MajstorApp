using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.Models;
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class BrowseController : Controller
    {
        private readonly ITvrtkaService _tvrtke;
        private readonly IVrstaRadaService _vrste;
        private readonly ILokacijaService _lokacije;
        private readonly ITvrtkaLokacijaService _tvrtkaLokacije; // relacija M-N

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

        // KLIJENT: pretraga/filter
        public async Task<IActionResult> Index(int? lokacijaId, int? vrstaRadaId)
        {
            // dropdown podaci (uzmi sve - za browse je ok)
            var lokacije = await _lokacije.GetAllAsync(q: null, page: 1, pageSize: 1000);
            var vrste = await _vrste.GetAllAsync(tvrtkaId: null, page: 1, pageSize: 1000);
            var tvrtke = await _tvrtke.GetAllAsync(q: null, page: 1, pageSize: 1000);

            // filter po vrsti rada: uzmi tvrtke koje imaju tu vrstu rada
            if (vrstaRadaId.HasValue)
            {
                var vrsta = vrste.FirstOrDefault(v => v.Id == vrstaRadaId.Value);
                if (vrsta != null)
                    tvrtke = tvrtke.Where(t => t.Id == vrsta.TvrtkaId).ToList();
                else
                    tvrtke = new List<MajstorFinder.DAL.Models.Tvrtka>();
            }

            // filter po lokaciji: treba relacija tvrtka-lokacija
            if (lokacijaId.HasValue)
            {
                var tvrtkaIds = await _tvrtkaLokacije.GetLokacijeIdsForTvrtkaAsync(lokacijaId.Value);
                tvrtke = tvrtke.Where(t => tvrtkaIds.Contains(t.Id)).ToList();
            }
            
            // map u VM (dok ne uvedemo AutoMapper)
            var vm = new BrowseVm
            {
                LokacijaId = lokacijaId,
                VrstaRadaId = vrstaRadaId,
                Lokacije = lokacije.Select(ToLokacijaVm).ToList(),
                VrsteRada = vrste.Select(ToVrstaRadaVm).ToList(),
                Tvrtke = tvrtke.Select(ToTvrtkaVm).ToList()
            };

            return View(vm);
        }

        // KLIJENT: detalji tvrtke + vrste rada + lokacije
        public async Task<IActionResult> Details(int id)
        {
            var tvrtka = await _tvrtke.GetByIdAsync(id);
            if (tvrtka == null) return NotFound();

            var vrste = await _vrste.GetByTvrtkaAsync(id);

            var lokacijaIds = await _tvrtkaLokacije.GetLokacijeIdsForTvrtkaAsync(id);
            var lokacije = await _lokacije.GetByIdAsync(lokacijaIds);

            ViewBag.Vrste = vrste.Select(ToVrstaRadaVm).ToList();
            ViewBag.Lokacije = lokacije.Select(ToLokacijaVm).ToList();

            return View(ToTvrtkaVm(tvrtka));
        }

        // ===== map metode (privremeno) =====
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