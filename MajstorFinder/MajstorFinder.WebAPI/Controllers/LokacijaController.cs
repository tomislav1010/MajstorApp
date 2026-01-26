using MajstorFinder.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.DAL.DbContext;
using MajstorFinder.DAL.DBC;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LokacijaController : ControllerBase
    {
        private readonly MajstoriDbContext _context;
        public LokacijaController(MajstoriDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Get(string? search, int page = 1, int count = 10)
        {
            var q = _context.Lokacijas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Name.Contains(search));

            var items = q.Skip((page - 1) * count).Take(count).ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.Lokacijas.Find(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult Create(LokacijaCreateDto dto)
        {
            if (_context.Lokacijas.Any(l => l.Name == dto.Name))
                return BadRequest("Lokacija s istim nazivom već postoji.");

            var entity = new MajstorFinder.DAL.Models.Lokacija { Name = dto.Name };
            _context.Lokacijas.Add(entity);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, LokacijaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");

            var entity = _context.Lokacijas.Find(id);
            if (entity == null) return NotFound();

            if (_context.Lokacijas.Any(l => l.Id != id && l.Name == dto.Name))
                return BadRequest("Lokacija s istim nazivom već postoji.");

            entity.Name = dto.Name;
            _context.SaveChanges();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.Lokacijas.Find(id);
            if (entity == null) return NotFound();

            try
            {
                _context.Lokacijas.Remove(entity);
                _context.SaveChanges();
                return NoContent();
            }
            catch
            {
                return Conflict("Ne mogu obrisati: Lokacija je povezana s tvrtkama.");
            }
        }
    }
}
