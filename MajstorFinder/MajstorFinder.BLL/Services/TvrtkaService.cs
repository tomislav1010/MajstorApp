using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;
using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.BLL.Services
{
    public class TvrtkaService : ITvrtkaService
    {
        private readonly MajstoriDbContext _db;
        public TvrtkaService(MajstoriDbContext db) => _db = db;

        public async Task<List<Tvrtka>> GetAllAsync(string? q, int page, int pageSize)
        {
            var query = _db.Tvrtkas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Name.Contains(q));

            return await query
                .OrderBy(t => t.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(string? q)
        {
            var query = _db.Tvrtkas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(t => t.Name.Contains(q));

            return await query.CountAsync();
        }

        public Task<Tvrtka?> GetByIdAsync(int id)
            => _db.Tvrtkas.FirstOrDefaultAsync(t => t.Id == id);

        public async Task<Tvrtka> CreateAsync(Tvrtka tvrtka)
        {
            // osnovna validacija
            if (string.IsNullOrWhiteSpace(tvrtka.Name))
                throw new ArgumentException("Naziv je obavezan.");

            _db.Tvrtkas.Add(tvrtka);
            await _db.SaveChangesAsync();
            return tvrtka;
        }

        public async Task<bool> UpdateAsync(int id, Tvrtka tvrtka)
        {
            var existing = await _db.Tvrtkas.FindAsync(id);
            if (existing == null) return false;

            existing.Name = tvrtka.Name;
            existing.Description = tvrtka.Description;
            existing.Phone = tvrtka.Phone;
            existing.Email = tvrtka.Email;

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _db.Tvrtkas.FindAsync(id);
            if (existing == null) return false;

            _db.Tvrtkas.Remove(existing);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<Lokacija>> GetLokacijeAsync(int tvrtkaId)
        {
            // Učitaj tvrtku zajedno s lokacijama (M-N veza)
            return await _db.TvrtkaLokacijas
                .Where(x => x.TvrtkaId == tvrtkaId)
                .Select(x => x.Lokacija)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }
    }
}