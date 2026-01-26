using MajstorFinder.BLL.DTOs;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.DAL.DBC;

namespace MajstorFinder.BLL.Services
{
    public class ZahtjevService : IZahtjevService
    {
        private readonly MajstoriDbContext _db;

        public ZahtjevService(MajstoriDbContext db)
        {
            _db = db;
        }

        public async Task<int> CreateAsync(int korisnikid, CreateZahtijevDto dto)
        {
            var z = new Zahtjev
            {
                KorisnikId = korisnikid,
                TvrtkaId = dto.TvrtkaId,
                VrstaRadaId = dto.VrstaRadaId,
                Description = dto.Description,
                Status = "Novi",
                DateCreated = DateTime.Now
            };
            _db.Zahtjevs.Add(z);
            await _db.SaveChangesAsync();
            return z.Id;
        }

        public async Task DeleteAsync(int id)
        {
            var z=await _db.Zahtjevs.FirstOrDefaultAsync(x => x.Id == id);
            if (z == null) return;
            _db.Zahtjevs.Remove(z);
            await _db.SaveChangesAsync();
        }

        public Task<List<Zahtjev>> GetAllAsync()
        =>_db.Zahtjevs
            .OrderByDescending(z=>z.DateCreated)
            .ToListAsync();

        public Task<Zahtjev> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Zahtjev>> GetByKorisnikAsync(int korisnikId)
        =>_db.Zahtjevs
            .Where(z=>z.KorisnikId==korisnikId)
            .OrderByDescending (z=>z.DateCreated)
            .ToListAsync();

        public Task UpdateAsync(int id, string status)
        {
            throw new NotImplementedException();
        }
    }
}
