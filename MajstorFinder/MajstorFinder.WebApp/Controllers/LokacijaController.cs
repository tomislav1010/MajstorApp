using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class LokacijaController : AdminController
    {
        private readonly IHttpClientFactory _factory;
        public LokacijaController(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Api()
        {
            var jwt = HttpContext.Session.GetString("jwt");
            return ApiClientFactory.CreateWithJwt(_factory, jwt);
        }

        /*public async Task<IActionResult> Index(string? q)
        {
            var client = Api();
            var lokacije = await client.GetFromJsonAsync<List<LokacijaVm>>("/api/Lokacija") ?? new();

            if (!string.IsNullOrWhiteSpace(q))
                lokacije = lokacije.Where(l => l.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Q = q ?? "";
            return View(lokacije);
        }*/

        [HttpGet]
        public IActionResult Create() => View(new LokacijaVm());

        [HttpPost]
        public async Task<IActionResult> Create(LokacijaVm model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = Api();
            var resp = await client.PostAsJsonAsync("/api/Lokacija", new { name = model.Name });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri spremanju (možda duplikat).");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = Api();
            var lok = await client.GetFromJsonAsync<LokacijaVm>($"/api/Lokacija/{id}");
            if (lok == null) return NotFound();
            return View(lok);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, LokacijaVm model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var client = Api();
            var resp = await client.PutAsJsonAsync($"/api/Lokacija/{id}", new { id = model.Id, name = model.Name });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju (možda duplikat).");
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = Api();
            var resp = await client.DeleteAsync($"/api/Lokacija/{id}");

            if (!resp.IsSuccessStatusCode)
                TempData["Err"] = "Ne mogu obrisati lokaciju (možda je vezana uz tvrtke).";

            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Index(string? q, int page = 1, int pageSize = 5)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var list = await client.GetFromJsonAsync<List<LokacijaVm>>("/api/Lokacija") ?? new();

            if (!string.IsNullOrWhiteSpace(q))
                list = list.Where(x => x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            var total = list.Count;
            var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Q = q ?? "";

            return View(items);
        }


        [HttpDelete]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            var res = await client.DeleteAsync($"/api/Lokacija/{id}");

            if (!res.IsSuccessStatusCode)
                return BadRequest(await res.Content.ReadAsStringAsync());

            return Ok();
        }
    }
}
