using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class AuthController:Controller
    {
        private readonly IHttpClientFactory _factory;
        public AuthController(IHttpClientFactory factory) => _factory = factory;

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _factory.CreateClient("ApiClient");

            var resp = await client.PostAsJsonAsync("/api/user/login", new
            {
                email = model.Email,
                password = model.Password
            });

            if (!resp.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Pogrešan email ili lozinka.");
                return View(model);
            }

            var data = await resp.Content.ReadFromJsonAsync<TokenResponse>();
            HttpContext.Session.SetString("jwt", data!.Token);

            return RedirectToAction("Index", "Home");
        }
        public IActionResult AdminLoginPreview()
        {
            return View("Auth");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("jwt");
            return RedirectToAction("Login");
        }

        private class TokenResponse
        {
            public string Token { get; set; } = "";
        }
    }
}
