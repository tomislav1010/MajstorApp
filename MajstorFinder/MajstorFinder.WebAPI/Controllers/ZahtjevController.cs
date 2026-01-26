using MajstorFinder.WebAPI.DTOs;
using MajstorFinder.DAL.Models;
using Microsoft.AspNetCore.Mvc;
using MajstorFinder.DAL.DbContext;
using MajstorFinder.DAL.DBC;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ZahtjevController : ControllerBase
    {
        private readonly MajstoriDbContext _context;
        public ZahtjevController(MajstoriDbContext context) => _context = context;

        // GET: /api/Zahtjev?korisnikId=1&page=1&count=50
        [HttpGet]
        public IActionResult Get(int? korisnikId, int page = 1, int count = 50)
        {
            var q = _context.Zahtjevs.AsQueryable();

            if (korisnikId.HasValue)
                q = q.Where(z => z.KorisnikId == korisnikId.Value);

            var result = q
                .OrderByDescending(z => z.DateCreated)
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var z = _context.Zahtjevs.Find(id);
            if (z == null) return NotFound();
            return Ok(z);
        }

        [HttpPost]
        public IActionResult Create([FromBody] ZahtjevCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var z = new Zahtjev
            {
                Description = dto.Description,
                Status = dto.Status,
                KorisnikId = dto.KorisnikId,
                TvrtkaId = dto.TvrtkaId,
                VrstaRadaId = dto.VrstaRadaId,
                DateCreated = DateTime.Now
            };

            _context.Zahtjevs.Add(z);
            _context.SaveChanges();

            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.Now,
                Level = "INFO",
                Message = $"Zahtjev id={z.Id} kreiran (korisnikId={z.KorisnikId})."
            });
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = z.Id }, z);
        }


        [HttpPut("{id}/status")]
        public IActionResult UpdateStatus(int id, [FromBody] StatusDto dto)
        {
            var z = _context.Zahtjevs.SingleOrDefault(x => x.Id == id);
            if (z == null) return NotFound();

            z.Status = dto.Status;
            _context.SaveChanges();

            return Ok();
        }

        public class StatusDto
        {
            public string Status { get; set; } = "";
        }
    }
}

