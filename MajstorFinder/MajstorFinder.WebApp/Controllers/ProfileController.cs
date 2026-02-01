using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.BLL.DTOs;
using Microsoft.AspNetCore.Mvc;


namespace MajstorFinder.WebApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _users;
        public ProfileController(IUserService users) => _users = users;

        // ========== REGISTRACIJA (GET) ==========
        [HttpGet]
        public IActionResult Create()
        {
            // ako si već logiran, nema smisla registracija
            var userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId > 0) return RedirectToAction(nameof(Index));

            return View(new CreateUserVm());
        }

        // ========== REGISTRACIJA (POST) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserVm model)
        {
            var userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId > 0) return RedirectToAction(nameof(Index));

            if (!ModelState.IsValid) return View(model);

            // VM -> DTO (BLL ne smije znati za WebApp modele)
            var dto = new CreateUserDto
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password,
                IsAdmin = model.IsAdmin
            };

            var ok = await _users.CreateAsync(dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Korisničko ime ili email već postoji.");
                return View(model);
            }

            TempData["Ok"] = "Račun je kreiran. Sada se prijavite.";
            return RedirectToAction("Login", "Auth");
        }

        // ========== PROFIL (GET) ==========
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0) return RedirectToAction("Login", "Auth");

            var u = await _users.GetByIdAsync(userId);
            if (u == null) return RedirectToAction("Logout", "Auth");

            var dto = new UpdateUserDto
            {
                Username = u.Username,
                Email = u.Email
            };

            return View(dto);
        }

        // ========== PROFIL (POST) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(UpdateUserDto dto)
        {
            var userId = HttpContext.Session.GetInt32("userId") ?? 0;
            if (userId == 0) return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid) return View(dto);

            var ok = await _users.UpdateAsync(userId, dto);
            if (!ok)
            {
                ModelState.AddModelError("", "Email/username već postoji ili korisnik ne postoji.");
                return View(dto);
            }

            TempData["Ok"] = "Profil ažuriran.";
            return RedirectToAction(nameof(Index));
        }
    }
}