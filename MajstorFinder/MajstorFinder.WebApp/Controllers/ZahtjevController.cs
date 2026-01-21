using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            var mine = await client.GetFromJsonAsync<List<ZahtjevVm>>("/api/Zahtjev?korisnikId=1") ?? new();
            return View(mine);
        }


        public async Task<IActionResult> AdminIndex()
        {
            var client = Api();
            var list = await client.GetFromJsonAsync<List<ZahtjevVm>>("/api/Zahtjev") ?? new();
            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var client = Api();

            var res = await client.PutAsJsonAsync($"/api/Zahtjev/{id}/status", new { status });

            if (!res.IsSuccessStatusCode)
                TempData["Err"] = await res.Content.ReadAsStringAsync();

            return RedirectToAction(nameof(AdminIndex));
        }





    }
}
