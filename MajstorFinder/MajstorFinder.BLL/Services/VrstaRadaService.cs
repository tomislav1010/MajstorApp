using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;     // promijeni ako treba
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.BLL.Services;

public class VrstaRadaService : IVrstaRadaService
{
    private readonly MajstoriDbContext _db;
    public VrstaRadaService(MajstoriDbContext db) => _db = db;

    public async Task<List<VrstaRada>> GetAllAsync(int? tvrtkaId, int page, int pageSize)
    {
        var query = _db.VrstaRadas.AsQueryable();

        if (tvrtkaId.HasValue)
            query = query.Where(v => v.TvrtkaId == tvrtkaId.Value);

        return await query
            .OrderBy(v => v.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(int? tvrtkaId)
    {
        var query = _db.VrstaRadas.AsQueryable();

        if (tvrtkaId.HasValue)
            query = query.Where(v => v.TvrtkaId == tvrtkaId.Value);

        return await query.CountAsync();
    }

    public Task<VrstaRada?> GetByIdAsync(int id)
        => _db.VrstaRadas.FirstOrDefaultAsync(v => v.Id == id);

    public async Task<VrstaRada> CreateAsync(VrstaRada vrsta)
    {
        if (string.IsNullOrWhiteSpace(vrsta.Name))
            throw new ArgumentException("Naziv je obavezan.");

        _db.VrstaRadas.Add(vrsta);
        await _db.SaveChangesAsync();
        return vrsta;
    }

    public async Task<bool> UpdateAsync(int id, VrstaRada vrsta)
    {
        var existing = await _db.VrstaRadas.FindAsync(id);
        if (existing == null) return false;

        existing.Name = vrsta.Name;
        existing.TvrtkaId = vrsta.TvrtkaId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.VrstaRadas.FindAsync(id);
        if (existing == null) return false;

        _db.VrstaRadas.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    public Task<List<VrstaRada>> GetByTvrtkaAsync(int tvrtkaid)
    =>_db.VrstaRadas.Where(x=>x.TvrtkaId == tvrtkaid).ToListAsync();
}
