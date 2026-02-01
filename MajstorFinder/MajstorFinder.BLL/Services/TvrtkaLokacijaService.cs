using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;




using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.BLL.Services
{
    public class TvrtkaLokacijaService : ITvrtkaLokacijaService
    {
        private readonly MajstoriDbContext _db;
        public TvrtkaLokacijaService(MajstoriDbContext db) => _db = db;

        public async Task<List<Lokacija>> GetLokacijeForTvrtkaAsync(int tvrtkaId)
        {
            // vrati lokacije za tvrtku preko join tablice
            return await _db.TvrtkaLokacijas
                .Where(x => x.TvrtkaId == tvrtkaId)
                .Select(x => x.Lokacija)          // radi ako imaš navigation Lokacija
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task AddAsync(int tvrtkaId, int lokacijaId)
        {
            // spriječi duplikate
            bool exists = await _db.TvrtkaLokacijas
                .AnyAsync(x => x.TvrtkaId == tvrtkaId && x.LokacijaId == lokacijaId);

            if (exists) return;

            _db.TvrtkaLokacijas.Add(new TvrtkaLokacija
            {
                TvrtkaId = tvrtkaId,
                LokacijaId = lokacijaId
            });

            await _db.SaveChangesAsync();
        }

        public async Task RemoveAsync(int tvrtkaId, int lokacijaId)
        {
            var link = await _db.TvrtkaLokacijas
                .FirstOrDefaultAsync(x => x.TvrtkaId == tvrtkaId && x.LokacijaId == lokacijaId);

            if (link == null) return;

            _db.TvrtkaLokacijas.Remove(link);
            await _db.SaveChangesAsync();
        }

        public async Task SetLokacijeForTvrtkaAsync(int tvrtkaId, IEnumerable<int> lokacijaIds)
        {
            // target set (što korisnik želi)
            var desired = (lokacijaIds ?? Array.Empty<int>())
                .Where(id => id > 0)
                .Distinct()
                .ToHashSet();

            // current set (što je u bazi)
            var current = await _db.TvrtkaLokacijas
                .Where(x => x.TvrtkaId == tvrtkaId)
                .Select(x => x.LokacijaId)
                .ToListAsync();

            var currentSet = current.ToHashSet();

            // ADD: desired - current
            foreach (var lokId in desired.Except(currentSet))
            {
                _db.TvrtkaLokacijas.Add(new TvrtkaLokacija
                {
                    TvrtkaId = tvrtkaId,
                    LokacijaId = lokId
                });
            }

            // REMOVE: current - desired
            var toRemoveIds = currentSet.Except(desired).ToList();
            if (toRemoveIds.Count > 0)
            {
                var removeLinks = await _db.TvrtkaLokacijas
                    .Where(x => x.TvrtkaId == tvrtkaId && toRemoveIds.Contains(x.LokacijaId))
                    .ToListAsync();

                _db.TvrtkaLokacijas.RemoveRange(removeLinks);
            }

            await _db.SaveChangesAsync();
        }

        public async Task<List<TvrtkaLokacija>> GetByIdsAsync(int tvrtkaId, IEnumerable<int> lokacijasIds)
        {
            var ids = lokacijasIds?.Distinct().ToList() ?? new List<int>();
            return await _db.TvrtkaLokacijas
                .Where(x => x.TvrtkaId == tvrtkaId)
                .ToListAsync();
        }

        public async Task<List<int>> GetLokacijeIdsForTvrtkaAsync(int tvrtkaId)
        {
            return await _db.Set<Dictionary<string, object>>("TvrtkaLokacija")
                .Where(x => (int)x["TvrtkaId"] == tvrtkaId)
                .Select(x => (int)x["LokacijaId"])
                .ToListAsync();
        }
    }
}
