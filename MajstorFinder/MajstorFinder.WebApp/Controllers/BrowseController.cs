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
        private readonly ILokacijaService _lokacije;
        private readonly IVrstaRadaService _vrste;

        public BrowseController(ITvrtkaService tvrtke, ILokacijaService lokacije, IVrstaRadaService vrste)
        {
            _tvrtke = tvrtke;
            _lokacije = lokacije;
            _vrste = vrste;
        }

        // KLIJENT: browse + filter
        public async Task<IActionResult> Index(int? lokacijaId, int? vrstaRadaId)
        {
            // dropdown podaci
            var lokacije = await _lokacije.GetAllAsync(null, 1, 1000);
            var vrste = await _vrste.GetAllAsync(null, 1, 1000);

            // tvrtke (za minimum uzmemo sve)
            var tvrtke = await _tvrtke.GetAllAsync(null, 1, 2000);

            // filter po vrsti rada
            if (vrstaRadaId.HasValue)
            {
                tvrtke = tvrtke
                    .Where(t => vrste.Any(v => v.Id == vrstaRadaId.Value && v.TvrtkaId == t.Id))
                    .ToList();
            }

            // filter po lokaciji (koristi ITvrtkaService.GetLokacijeAsync)
            if (lokacijaId.HasValue)
            {
                var filtered = new List<Tvrtka>();

                foreach (var t in tvrtke)
                {
                    var loks = await _tvrtke.GetLokacijeAsync(t.Id);
                    if (loks.Any(l => l.Id == lokacijaId.Value))
                        filtered.Add(t);
                }

                tvrtke = filtered;
            }

            var vm = new BrowseVm
            {
                LokacijaId = lokacijaId,
                VrstaRadaId = vrstaRadaId,
                Lokacije = lokacije.Select(l => new LokacijaVm { Id = l.Id, Name = l.Name }).ToList(),
                VrsteRada = vrste.Select(v => new VrstaRadaVm { Id = v.Id, Name = v.Name, TvrtkaId = v.TvrtkaId }).ToList(),
                Tvrtke = tvrtke.Select(t => new TvrtkaVm
                {
                    Id = t.Id,
                    Name = t.Name,
                    Description = t.Description,
                    Phone = t.Phone,
                    Email = t.Email
                }).ToList()
            };

            return View(vm);
        }

        // KLIJENT: details tvrtke
        public async Task<IActionResult> Details(int id)
        {
            var tvrtka = await _tvrtke.GetByIdAsync(id);
            if (tvrtka == null) return NotFound();

            var vrste = await _vrste.GetByTvrtkaAsync(id);
            var lokacije = await _tvrtke.GetLokacijeAsync(id);

            ViewBag.Vrste = vrste.Select(v => new VrstaRadaVm { Id = v.Id, Name = v.Name, TvrtkaId = v.TvrtkaId }).ToList();
            ViewBag.Lokacije = lokacije.Select(l => new LokacijaVm { Id = l.Id, Name = l.Name }).ToList();

            var vm = new TvrtkaVm
            {
                Id = tvrtka.Id,
                Name = tvrtka.Name,
                Description = tvrtka.Description,
                Phone = tvrtka.Phone,
                Email = tvrtka.Email
            };

            return View(vm);
        }
    }
}