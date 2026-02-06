using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using System.Linq;

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
        public IActionResult Add([FromQuery] int tvrtkaId, [FromQuery] int lokacijaId)
        {
            // provjere da postoje
            if (!_context.Tvrtkas.Any(t => t.Id == tvrtkaId))
                return NotFound("TvrtkaId ne postoji.");

            if (!_context.Lokacijas.Any(l => l.Id == lokacijaId))
                return NotFound("LokacijaId ne postoji.");

            // provjera duplikata veze
            bool exists = _context.TvrtkaLokacijas
                .Any(x => x.TvrtkaId == tvrtkaId && x.LokacijaId == lokacijaId);

            if (exists)
                return Conflict("Veza već postoji.");

            _context.TvrtkaLokacijas.Add(new TvrtkaLokacija
            {
                TvrtkaId = tvrtkaId,
                LokacijaId = lokacijaId
            });

            _context.SaveChanges();

            return Ok("Veza dodana.");
        }

        // DELETE: /api/tvrtka-lokacija?tvrtkaId=1&lokacijaId=2
        [HttpDelete]
        public IActionResult Remove([FromQuery] int tvrtkaId, [FromQuery] int lokacijaId)
        {
            var link = _context.TvrtkaLokacijas
                .FirstOrDefault(x => x.TvrtkaId == tvrtkaId && x.LokacijaId == lokacijaId);

            if (link == null)
                return NotFound("Veza ne postoji.");

            _context.TvrtkaLokacijas.Remove(link);
            _context.SaveChanges();

            return NoContent();
        }

        // (opcionalno) GET: /api/tvrtka-lokacija?tvrtkaId=1
        // vrati sve lokacije za tvrtku
        [HttpGet]
        public IActionResult GetForTvrtka([FromQuery] int tvrtkaId)
        {
            if (!_context.Tvrtkas.Any(t => t.Id == tvrtkaId))
                return NotFound("TvrtkaId ne postoji.");

            var lokacije = _context.TvrtkaLokacijas
                .AsNoTracking()
                .Where(x => x.TvrtkaId == tvrtkaId)
                .Include(x => x.Lokacija)
                .OrderBy(x => x.Lokacija.Name)
                .Select(x => new { x.LokacijaId, x.Lokacija.Name })
                .ToList();

            return Ok(lokacije);
        }
    }
}
