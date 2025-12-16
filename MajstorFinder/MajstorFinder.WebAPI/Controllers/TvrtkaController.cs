using MajstorFinder.WebAPI.Models;
using Microsoft.AspNetCore.Mvc;

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



        [HttpPost]
        public IActionResult Create(Tvrtka tvrtka)
        {
            _context.Tvrtkas.Add(tvrtka);
            _context.SaveChanges();

            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.Now,
                Level = "INFO",
                Message = $"Tvrtka s id={tvrtka.Id} je stvorena."
            });
            _context.SaveChanges();

            return Ok(tvrtka);
        }



        [HttpPut("{id}")]
        public IActionResult Update(int id, Tvrtka tvrtka)
        {
            if (id != tvrtka.Id)
                return BadRequest();

            _context.Tvrtkas.Update(tvrtka);
            _context.SaveChanges();

            _context.Logs.Add(new Log
            {
                Timestamp = DateTime.Now,
                Level = "INFO",
                Message = $"Tvrtka s id={id} je ažurirana."
            });
            _context.SaveChanges();

            return Ok(tvrtka);
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
        }
    }
}
