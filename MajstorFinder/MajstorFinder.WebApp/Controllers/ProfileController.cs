using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IHttpClientFactory _factory;
        public ProfileController(IHttpClientFactory factory) => _factory = factory;

        private HttpClient Api()
        {
            var jwt = HttpContext.Session.GetString("jwt");
            return ApiClientFactory.CreateWithJwt(_factory, jwt);
        }

        public async Task<IActionResult> Index()
        {
            var client = Api();

            // ako imaš endpoint /api/User/me (idealno)
            var me = await client.GetFromJsonAsync<AppUserVm>("/api/User/me");
            return View(me);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAjax([FromBody] AppUserVm model)
        {
            var client = Api();
            var res = await client.PutAsJsonAsync("/api/User/me", model);

            if (!res.IsSuccessStatusCode)
                return BadRequest(await res.Content.ReadAsStringAsync());

            return Ok("Spremljeno");
        }
    }
}
