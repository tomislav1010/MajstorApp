using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class TvrtkaController : AdminController
    {
        private readonly IHttpClientFactory _factory;

        public TvrtkaController(IHttpClientFactory factory)
        {
            _factory = factory;
        }

        // LIST (za danas - bez paging prvo, pa dodamo)
        public async Task<IActionResult> Index(string? q)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            // Ako tvoj API ima search/paging endpoint, kasnije ćemo ga koristiti.
            var tvrtke = await client.GetFromJsonAsync<List<TvrtkaVm>>("/api/tvrtka");

            if (!string.IsNullOrWhiteSpace(q))
                tvrtke = tvrtke!
                    .Where(t => t.Name.Contains(q, StringComparison.OrdinalIgnoreCase))
                    .ToList();

            ViewBag.Q = q ?? "";
            return View(tvrtke ?? new List<TvrtkaVm>());
        }

        [HttpGet]
        public IActionResult Create() => View(new TvrtkaVm());

        [HttpPost]
        public async Task<IActionResult> Create(TvrtkaVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var resp = await client.PostAsJsonAsync("/api/tvrtka", new
            {
                name = model.Name,
                description = model.Description,
                phone = model.Phone,
                email = model.Email
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri spremanju tvrtke.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var tvrtka = await client.GetFromJsonAsync<TvrtkaVm>($"/api/tvrtka/{id}");
            if (tvrtka == null) return NotFound();

            return View(tvrtka);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, TvrtkaVm model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var resp = await client.PutAsJsonAsync($"/api/tvrtka/{id}", new
            {
                id = model.Id,
                name = model.Name,
                description = model.Description,
                phone = model.Phone,
                email = model.Email
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju tvrtke.");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var resp = await client.DeleteAsync($"/api/tvrtka/{id}");

            if (!resp.IsSuccessStatusCode)
            {
                // najčešće 409 ako je vezano za nešto
                TempData["Err"] = "Ne mogu obrisati tvrtku (možda je vezana uz vrste rada/zahtjeve).";
            }

            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Lokacije(int id)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var tvrtka = await client.GetFromJsonAsync<TvrtkaVm>($"/api/Tvrtka/{id}");
            if (tvrtka == null) return NotFound();

            var sveLokacije = await client.GetFromJsonAsync<List<LokacijaVm>>("/api/Lokacija") ?? new();
            var postojece = await client.GetFromJsonAsync<List<LokacijaVm>>($"/api/Tvrtka/{id}/lokacije") ?? new();

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
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            // current from API
            var postojece = await client.GetFromJsonAsync<List<LokacijaVm>>($"/api/Tvrtka/{model.TvrtkaId}/lokacije") ?? new();
            var current = postojece.Select(x => x.Id).ToHashSet();

            var desired = model.Lokacije.Where(x => x.Selected).Select(x => x.Id).ToHashSet();

            // add new links
            foreach (var lokId in desired.Except(current))
                await client.PostAsync($"/api/tvrtka-lokacija?tvrtkaId={model.TvrtkaId}&lokacijaId={lokId}", null);

            // remove unchecked
            foreach (var lokId in current.Except(desired))
                await client.DeleteAsync($"/api/tvrtka-lokacija?tvrtkaId={model.TvrtkaId}&lokacijaId={lokId}");

            return RedirectToAction("Index");
        }
    }
}
