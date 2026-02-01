using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.BLL.Interfaces;


using MajstorFinder.BLL.Interfaces;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;
        private readonly IKorisnikService _korisnici;

        public AuthController(IAuthService auth, IKorisnikService korisnici)
        {
            _auth = auth;
            _korisnici = korisnici;
        }

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _auth.ValidateLogin(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Pogrešan email ili lozinka.");
                return View(model);
            }

            // ✅ KORISNIK TABLICA (za Zahtjev.KorisnikId)
            int korisnikId = await _korisnici.GetOrCreateKorisnikIdAsync(user.Email, user.Username);

            // SESSION (bez JWT-a)
            HttpContext.Session.SetInt32("userId", korisnikId);          // <-- BITNO: ovo je Korisnik.Id
            HttpContext.Session.SetInt32("appUserId", user.Id);          // opcionalno (ako ti zatreba)
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("isAdmin", user.IsAdmin ? "1" : "0");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // obriši ako ne koristiš
        public IActionResult AdminLoginPreview() => View("Auth");
    }
}