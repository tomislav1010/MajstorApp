using MajstorFinder.WebAPI.DTOs.MajstoriApp.WebAPI.Dtos;
using MajstorFinder.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MajstorFinder.WebAPI.DTOs;


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


        [HttpGet]
        public IActionResult Get(string? search, int page = 1, int count = 10)
        {
            var query = _context.Tvrtkas.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(t => t.Name.Contains(search));

            var result = query
                .Skip((page - 1) * count)
                .Take(count)
                .ToList();

            return Ok(result);
        }


        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var tvrtka = _context.Tvrtkas.Find(id);

            if (tvrtka == null)
                return NotFound();

            return Ok(tvrtka);
        }




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

         return CreatedAtAction(
         nameof(GetById),
         new { id = tvrtka.Id },
         tvrtka);
         AddLog("INFO", $"Tvrtka s id={tvrtka.Id} je stvorena.");

        }



        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TvrtkaUpdateDto dto)
        {
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

            return Ok(tvrtka);
            AddLog("INFO", $"Tvrtka s id={id} je ažurirana.");

        }


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var tvrtka = _context.Tvrtkas.Find(id);
            if (tvrtka == null)
                return NotFound();

            _context.Tvrtkas.Remove(tvrtka);
            _context.SaveChanges();

            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.Now,
                Level = "INFO",
                Message = $"Tvrtka s id={id} je obrisana."
            });
            _context.SaveChanges();

            return NoContent();
            AddLog("INFO", $"Tvrtka s id={id} je obrisana.");

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
