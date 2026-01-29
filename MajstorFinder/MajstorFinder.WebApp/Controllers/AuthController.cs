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
            if (!ModelState.IsValid) return View(model);

            var user = _auth.ValidateLogin(model.Email, model.Password);
            if (user == null)
            {
                ModelState.AddModelError("", "Pogrešan email ili lozinka.");
                return View(model);
            }

            // spremi user u session (umjesto jwt)
            HttpContext.Session.SetInt32("userId", user.Id);
            HttpContext.Session.SetString("username", user.Username);
            bool isAdmin = HttpContext.Session.GetString("isAdmin") == "1";

            // ako imaš u tablici ulogu/isAdmin, spremi i to
            // HttpContext.Session.SetString("role", user.Role);

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