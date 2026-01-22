using MajstorFinder.DAL.Models;
using MajstorFinder.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/tvrtka-lokacija")]
    public class TvrtkaLokacijaController : ControllerBase
    {
        private readonly MajstoriDbContext _context;
        public TvrtkaLokacijaController(MajstoriDbContext context) => _context = context;

        // POST: /api/tvrtka-lokacija?tvrtkaId=1&lokacijaId=2
        [HttpPost]
        public IActionResult Add(int tvrtkaId, int lokacijaId)
        {
            var tvrtka = _context.Tvrtkas
                .Include(t => t.Lokacijas)
                .SingleOrDefault(t => t.Id == tvrtkaId);

            if (tvrtka == null)
                return NotFound("TvrtkaId ne postoji.");

            var lokacija = _context.Lokacijas.Find(lokacijaId);
            if (lokacija == null)
                return NotFound("LokacijaId ne postoji.");

            if (tvrtka.Lokacijas.Any(l => l.Id == lokacijaId))
                return Conflict("Veza već postoji.");

            tvrtka.Lokacijas.Add(lokacija);
            _context.SaveChanges();

            return Ok("Veza dodana.");
        }

        // DELETE: /api/tvrtka-lokacija?tvrtkaId=1&lokacijaId=2
        [HttpDelete]
        public IActionResult Remove(int tvrtkaId, int lokacijaId)
        {
            var tvrtka = _context.Tvrtkas
                .Include(t => t.Lokacijas)
                .SingleOrDefault(t => t.Id == tvrtkaId);

            if (tvrtka == null)
                return NotFound("TvrtkaId ne postoji.");

            var lokacija = tvrtka.Lokacijas.SingleOrDefault(l => l.Id == lokacijaId);
            if (lokacija == null)
                return NotFound("Veza ne postoji.");

            tvrtka.Lokacijas.Remove(lokacija);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
