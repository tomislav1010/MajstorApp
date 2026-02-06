using MajstorFinder.WebAPI.DTOs;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.DAL.DBC;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VrstaRadaController : ControllerBase
    {
        private readonly MajstoriDbContext _context;
        public VrstaRadaController(MajstoriDbContext context) => _context = context;

        [HttpGet]
        public IActionResult Get(string? search, int page = 1, int count = 10)
        {
            var q = _context.VrstaRadas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(x => x.Name.Contains(search));

            var items = q.Skip((page - 1) * count).Take(count).ToList();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var item = _context.VrstaRadas.Find(id);
            return item == null ? NotFound() : Ok(item);
        }

        [HttpPost]
        public IActionResult Create(VrstaRadaCreateDto dto)
        {
            if (!_context.Tvrtkas.Any(t => t.Id == dto.TvrtkaId))
                return BadRequest("TvrtkaId ne postoji.");

            // (I4 minimum kasnije) – zabrana duplikata po tvrtki
            if (_context.VrstaRadas.Any(v => v.TvrtkaId == dto.TvrtkaId && v.Name == dto.Name))
                return BadRequest("Vrsta rada s istim nazivom već postoji za tu tvrtku.");

            var entity = new MajstorFinder.DAL.Models.VrstaRada
            {
                Name = dto.Name,
                TvrtkaId = dto.TvrtkaId
            };

            _context.VrstaRadas.Add(entity);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, entity);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, VrstaRadaUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch.");

            var entity = _context.VrstaRadas.Find(id);
            if (entity == null) return NotFound();

            if (!_context.Tvrtkas.Any(t => t.Id == dto.TvrtkaId))
                return BadRequest("TvrtkaId ne postoji.");

            if (_context.VrstaRadas.Any(v => v.Id != id && v.TvrtkaId == dto.TvrtkaId && v.Name == dto.Name))
                return BadRequest("Vrsta rada s istim nazivom već postoji za tu tvrtku.");

            entity.Name = dto.Name;
            entity.TvrtkaId = dto.TvrtkaId;

            _context.SaveChanges();
            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var entity = _context.VrstaRadas.Find(id);
            if (entity == null) return NotFound();

        
            try
            {
                _context.VrstaRadas.Remove(entity);
                _context.SaveChanges();
                return NoContent();
            }
            catch
            {
                return Conflict("Ne mogu obrisati: VrstaRada je povezana s drugim zapisima.");
            }
        }
    }
}
