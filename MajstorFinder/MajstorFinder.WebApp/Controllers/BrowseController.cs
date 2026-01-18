using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class BrowseController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public BrowseController(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Api()
        {
            var jwt = HttpContext.Session.GetString("jwt");
            return ApiClientFactory.CreateWithJwt(_factory, jwt);
        }

        public async Task<IActionResult> Index(int? lokacijaId, int? vrstaRadaId)
        {
            var client = Api();

            var lokacije = await client.GetFromJsonAsync<List<LokacijaVm>>("/api/Lokacija") ?? new();
            var vrste = await client.GetFromJsonAsync<List<VrstaRadaVm>>("/api/VrstaRada") ?? new();
            var tvrtke = await client.GetFromJsonAsync<List<TvrtkaVm>>("/api/Tvrtka") ?? new();

            // MINIMUM filter (brzo): filtriramo klijentski
            if (vrstaRadaId.HasValue)
                tvrtke = tvrtke.Where(t => vrste.Any(v => v.Id == vrstaRadaId && v.TvrtkaId == t.Id)).ToList();

            if (lokacijaId.HasValue)
            {
                // treba API endpoint /api/Tvrtka/{id}/lokacije da filtriramo po lokaciji
                // ako ga nema: preskoči lokacija filter ili ga implementiramo u API-u
            }

            var vm = new BrowseVm
            {
                LokacijaId = lokacijaId,
                VrstaRadaId = vrstaRadaId,
                Lokacije = lokacije,
                VrsteRada = vrste,
                Tvrtke = tvrtke
            };

            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = Api();
            var tvrtka = await client.GetFromJsonAsync<TvrtkaVm>($"/api/Tvrtka/{id}");
            if (tvrtka == null) return NotFound();

            // Učitaj vrste rada za tu tvrtku (iz svih vrsta rada filtriraj)
            var vrste = await client.GetFromJsonAsync<List<VrstaRadaVm>>("/api/VrstaRada") ?? new();
            ViewBag.Vrste = vrste.Where(v => v.TvrtkaId == id).ToList();

            return View(tvrtka);
        }


    }
}
