using MajstorFinder.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.BLL.Services
{
    public class KorisnikService : IKorisnikService
    {
        private readonly MajstoriDbContext _db;
        public KorisnikService(MajstoriDbContext db) => _db = db;

        public async Task<int> GetOrCreateKorisnikIdAsync(string email, string name)
        {
            // 1) probaj naći po emailu
            var k = await _db.Korisniks.FirstOrDefaultAsync(x => x.Email == email);
            if (k != null) return k.Id;

            // 2) ako ne postoji - kreiraj
            k = new Korisnik
            {
                Email = email,
                Name = string.IsNullOrWhiteSpace(name) ? email : name,
                Phone = "" // ako je NOT NULL u bazi
            };

            _db.Korisniks.Add(k);
            await _db.SaveChangesAsync();
            return k.Id;
        }
    }
}
