using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.BLL.Interfaces;


namespace MajstorFinder.WebApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth) => _auth = auth;

        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());


        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = _auth.ValidateLogin(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("", "Pogrešan email ili lozinka.");
                return View(model);
            }

            // SESSION (bez JWT-a, kako profesor traži)
            HttpContext.Session.SetInt32("userId", user.Id);
            HttpContext.Session.SetString("username", user.Username);
            HttpContext.Session.SetString("isAdmin", user.IsAdmin ? "1" : "0");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ovo možeš obrisati ako ti ne treba
        public IActionResult AdminLoginPreview() => View("Auth");
    }
}