using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.Services
{
    public class TvrtkaLokacijaService : ITvrtkaLokacijaService
    {
        private readonly MajstoriDbContext _db;
        public TvrtkaLokacijaService(MajstoriDbContext db) => _db = db;

        public async Task<List<Lokacija>> GetLokacijeForTvrtkaAsync(int tvrtkaId)
        {
            return await _db.Tvrtkas
                .Where(t => t.Id == tvrtkaId)
                .SelectMany(t => t.Lokacijas)
                .ToListAsync();
        }

        public async Task SetLokacijeForTvrtkaAsync(int tvrtkaId, IEnumerable<int> lokacijaIds)
        {
            var tvrtka = await _db.Tvrtkas
                .Include(t => t.Lokacijas)
                .FirstOrDefaultAsync(t => t.Id == tvrtkaId);

            if (tvrtka == null) return;

            var desired = lokacijaIds.Distinct().ToHashSet();

            // učitaj sve lokacije koje trebaju biti vezane
            var lokacije = await _db.Lokacijas
                .Where(l => desired.Contains(l.Id))
                .ToListAsync();

            // resetiraj veze (najjednostavnije)
            tvrtka.Lokacijas.Clear();
            foreach (var l in lokacije)
                tvrtka.Lokacijas.Add(l);

            await _db.SaveChangesAsync();
        }
    }
}