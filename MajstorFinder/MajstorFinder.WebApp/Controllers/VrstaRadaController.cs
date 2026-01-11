using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class VrstaRadaController : AdminController
    {
        private readonly IHttpClientFactory _factory;
        public VrstaRadaController(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Api()
        {
            var jwt = HttpContext.Session.GetString("jwt");
            return ApiClientFactory.CreateWithJwt(_factory, jwt);
        }

        // LIST
       /* public async Task<IActionResult> Index(int? tvrtkaId)
        {
            var client = Api();

            // 1) sve tvrtke za dropdown filter
            var tvrtke = await client.GetFromJsonAsync<List<TvrtkaVm>>("/api/Tvrtka");
            ViewBag.Tvrtke = tvrtke ?? new List<TvrtkaVm>();
            ViewBag.SelectedTvrtkaId = tvrtkaId;

            // 2) vrste rada (ako tvoj API nema poseban filter endpoint, filtriramo u MVC-u)
            var vrste = await client.GetFromJsonAsync<List<VrstaRadaVm>>("/api/VrstaRada");
            vrste ??= new List<VrstaRadaVm>();

            // mapiraj TvrtkaName radi prikaza
            var dict = (tvrtke ?? new List<TvrtkaVm>()).ToDictionary(x => x.Id, x => x.Name);
            foreach (var v in vrste)
                if (dict.TryGetValue(v.TvrtkaId, out var name)) v.TvrtkaName = name;

            if (tvrtkaId.HasValue)
                vrste = vrste.Where(v => v.TvrtkaId == tvrtkaId.Value).ToList();

            return View(vrste);
        }
       */
        // CREATE
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTvrtke();
            return View(new VrstaRadaVm());
        }

        [HttpPost]
        public async Task<IActionResult> Create(VrstaRadaVm model)
        {
            if (!ModelState.IsValid)
            {
                await LoadTvrtke(model.TvrtkaId);
                return View(model);
            }

            var client = Api();
            var resp = await client.PostAsJsonAsync("/api/VrstaRada", new
            {
                name = model.Name,
                tvrtkaId = model.TvrtkaId
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri spremanju vrste rada (možda duplikat).");
                await LoadTvrtke();
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // EDIT
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var client = Api();
            var vrsta = await client.GetFromJsonAsync<VrstaRadaVm>($"/api/VrstaRada/{id}");
            if (vrsta == null) return NotFound();

            await LoadTvrtke(vrsta.TvrtkaId);
            return View(vrsta);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, VrstaRadaVm model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await LoadTvrtke(model.TvrtkaId);
                return View(model);
            }

            var client = Api();
            var resp = await client.PutAsJsonAsync($"/api/VrstaRada/{id}", new
            {
                id = model.Id,
                name = model.Name,
                tvrtkaId = model.TvrtkaId
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Greška pri ažuriranju (možda duplikat).");
                await LoadTvrtke(model.TvrtkaId);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // DELETE
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = Api();
            var resp = await client.DeleteAsync($"/api/VrstaRada/{id}");

            if (!resp.IsSuccessStatusCode)
                TempData["Err"] = "Ne mogu obrisati vrstu rada (možda je vezana na zahtjeve).";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadTvrtke(int? selectedId = null)
        {
            var client = Api();
            var tvrtke = await client.GetFromJsonAsync<List<TvrtkaVm>>("/api/Tvrtka") ?? new List<TvrtkaVm>();
            ViewBag.Tvrtke = tvrtke;
            ViewBag.SelectedTvrtkaId = selectedId;
        }


        public async Task<IActionResult> Index(int? tvrtkaId, string? q, int page = 1, int pageSize = 5)
        {
            var jwt = HttpContext.Session.GetString("jwt");
            var client = ApiClientFactory.CreateWithJwt(_factory, jwt);

            // 1) tvrtke za dropdown
            var tvrtke = await client.GetFromJsonAsync<List<TvrtkaVm>>("/api/Tvrtka") ?? new();
            ViewBag.Tvrtke = tvrtke;
            ViewBag.SelectedTvrtkaId = tvrtkaId;

            // 2) vrste rada
            var list = await client.GetFromJsonAsync<List<VrstaRadaVm>>("/api/VrstaRada") ?? new();

            if (tvrtkaId.HasValue)
                list = list.Where(x => x.TvrtkaId == tvrtkaId.Value).ToList();

            if (!string.IsNullOrWhiteSpace(q))
                list = list.Where(x => x.Name.Contains(q, StringComparison.OrdinalIgnoreCase)).ToList();

            var total = list.Count;
            var items = list.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            ViewBag.Page = page;
            ViewBag.TotalPages = (int)Math.Ceiling(total / (double)pageSize);
            ViewBag.Q = q ?? "";

            return View(items);
        }
    }
}
