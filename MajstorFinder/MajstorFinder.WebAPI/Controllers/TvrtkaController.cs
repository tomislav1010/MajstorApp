using MajstorFinder.WebAPI.DTOs.MajstoriApp.WebAPI.Dtos;
using MajstorFinder.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MajstorFinder.WebAPI.DTOs;
using MajstorFinder.DAL.DBC;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TvrtkaController : ControllerBase
    {
        private readonly MajstoriDbContext _context;

        public TvrtkaController(MajstoriDbContext context)
        {
            _context = context;
        }

        // GET api/tvrtka?search=...&page=1&count=10
        [HttpGet]
        public IActionResult Get(string? search, int page = 1, int count = 10)
        {
            if (page < 1) page = 1;
            if (count < 1) count = 10;

            var query = _context.Tvrtkas.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(t => t.Name.Contains(search));

            var result = query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            return Ok(result);
        }

        // GET api/tvrtka/5
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var tvrtka = _context.Tvrtkas
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);

            if (tvrtka == null)
                return NotFound();

            return Ok(tvrtka);
        }

        // POST api/tvrtka
        [HttpPost]
        public IActionResult Create([FromBody] TvrtkaCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tvrtka = new Tvrtka
            {
                Name = dto.Name,
                Description = dto.Description,
                Phone = dto.Phone,
                Email = dto.Email
            };

            _context.Tvrtkas.Add(tvrtka);
            _context.SaveChanges();

            AddLog("INFO", $"Tvrtka s id={tvrtka.Id} je stvorena.");

            return CreatedAtAction(nameof(GetById), new { id = tvrtka.Id }, tvrtka);
        }

        // PUT api/tvrtka/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TvrtkaUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var tvrtka = _context.Tvrtkas.Find(id);
            if (tvrtka == null)
                return NotFound();

            tvrtka.Name = dto.Name;
            tvrtka.Description = dto.Description;
            tvrtka.Phone = dto.Phone;
            tvrtka.Email = dto.Email;

            _context.SaveChanges();

            AddLog("INFO", $"Tvrtka s id={id} je ažurirana.");

            return Ok(tvrtka);
        }

        // DELETE api/tvrtka/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var tvrtka = _context.Tvrtkas.Find(id);
            if (tvrtka == null)
                return NotFound();

            _context.Tvrtkas.Remove(tvrtka);
            _context.SaveChanges();

            AddLog("INFO", $"Tvrtka s id={id} je obrisana.");

            return NoContent();
        }

        // GET api/tvrtka/5/lokacije
        [HttpGet("{id}/lokacije")]
        public IActionResult GetLokacijeForTvrtka(int id)
        {
            // M:N preko TvrtkaLokacija
            var lokacije = _context.TvrtkaLokacijas
                .AsNoTracking()
                .Where(x => x.TvrtkaId == id)
                .Include(x => x.Lokacija)
                .OrderBy(x => x.Lokacija.Name)
                .Select(x => new { x.LokacijaId, x.Lokacija.Name })
                .ToList();

            // Ako želiš vratiti NotFound kad tvrtka ne postoji:
            if (!_context.Tvrtkas.Any(t => t.Id == id))
                return NotFound();

            return Ok(lokacije);
        }

        // GET api/tvrtka/5/vrsterada
        [HttpGet("{id}/vrsterada")]
        public IActionResult GetVrsteRadaForTvrtka(int id)
        {
            if (!_context.Tvrtkas.Any(t => t.Id == id))
                return NotFound();

            var vrste = _context.VrstaRadas
                .AsNoTracking()
                .Where(v => v.TvrtkaId == id)
                .Select(v => new { v.Id, v.Name, v.TvrtkaId })
                .OrderBy(v => v.Name)
                .ToList();

            return Ok(vrste);
        }

        // GET api/tvrtka/search?lokacijaId=1&vrstaRadaId=2
        [HttpGet("search")]
        public IActionResult Search([FromQuery] int? lokacijaId, [FromQuery] int? vrstaRadaId)
        {
            var q = _context.Tvrtkas
                .AsNoTracking()
                // M:N join
                .Include(t => t.TvrtkaLokacijas)
                    .ThenInclude(tl => tl.Lokacija)
                .Include(t => t.VrstaRadas)
                .AsQueryable();

            if (lokacijaId.HasValue)
                q = q.Where(t => t.TvrtkaLokacijas.Any(tl => tl.LokacijaId == lokacijaId.Value));

            if (vrstaRadaId.HasValue)
                q = q.Where(t => t.VrstaRadas.Any(v => v.Id == vrstaRadaId.Value));

            var result = q
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    t.Description,
                    t.Phone,
                    t.Email
                })
                .OrderBy(t => t.Name)
                .ToList();

            return Ok(result);
        }

        private void AddLog(string level, string message)
        {
            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = message
            });
            _context.SaveChanges();
        }
    }
}