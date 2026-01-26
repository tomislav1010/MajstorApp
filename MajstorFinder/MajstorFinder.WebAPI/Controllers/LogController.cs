using MajstorFinder.DAL.DbContext;
using Microsoft.AspNetCore.Mvc;

namespace MajstorFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/logs")]
    public class LogsController : ControllerBase
    {
        private readonly MajstoriDbContext _context;

        public LogsController(MajstoriDbContext context)
        {
            _context = context;
        }

        [HttpGet("get/{n}")]
        public IActionResult GetLast(int n)
        {
            return Ok(_context.Logs
                .OrderByDescending(l => l.Timestamp)
                .Take(n)
                .ToList());
        }

        [HttpGet("count")]
        public IActionResult Count()
        {
            return Ok(_context.Logs.Count());
        }
    }
}
