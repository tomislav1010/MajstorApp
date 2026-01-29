using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.WebApp.Helpers;
using MajstorFinder.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly IUserService _users;
        public ProfileController(IUserService users) => _users = users;

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
                Email = u.Email,
                // Phone = u.Phone
            };

            return View(dto);
        }

        [HttpPost]
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