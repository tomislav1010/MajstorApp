using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class ZahtjevController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public ZahtjevController(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Api()
        {
            var jwt = HttpContext.Session.GetString("jwt");
            return ApiClientFactory.CreateWithJwt(_factory, jwt);
        }

        [HttpGet]
        public IActionResult Create(int tvrtkaId, int vrstaRadaId)
        {
            return View(new ZahtjevCreateVm
            {
                TvrtkaId = tvrtkaId,
                VrstaRadaId = vrstaRadaId,
                KorisnikId = 1
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ZahtjevCreateVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = Api();
            var resp = await client.PostAsJsonAsync("/api/Zahtjev", new
            {
                description = model.Description,
                korisnikId = model.KorisnikId,
                tvrtkaId = model.TvrtkaId,
                vrstaRadaId = model.VrstaRadaId,
                status = "Poslano"
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri slanju zahtjeva.");
                return View(model);
            }

            return RedirectToAction(nameof(Moji));
        }

        public async Task<IActionResult> Moji()
        {
            var client = Api();
            var all = await client.GetFromJsonAsync<List<ZahtjevVm>>("/api/Zahtjev") ?? new();
            var mine = all.Where(z => z.KorisnikId == 1).ToList(); // ako nema KorisnikId u VM, dodaj ga

            return View(mine);
        }
    }
}
