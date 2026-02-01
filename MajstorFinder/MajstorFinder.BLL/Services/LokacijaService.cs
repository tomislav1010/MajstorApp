using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.BLL.Interfaces;
using MajstorFinder.DAL.DBC;     
using MajstorFinder.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace MajstorFinder.BLL.Services;

public class LokacijaService : ILokacijaService
{
    private readonly MajstoriDbContext _db;
    public LokacijaService(MajstoriDbContext db) => _db = db;

    public async Task<List<Lokacija>> GetAllAsync(string? q, int page, int pageSize)
    {
        var query = _db.Lokacijas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(l => l.Name.Contains(q));

        return await query
            .OrderBy(l => l.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountAsync(string? q)
    {
        var query = _db.Lokacijas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(l => l.Name.Contains(q));

        return await query.CountAsync();
    }

    public Task<Lokacija?> GetByIdAsync(int id)
        => _db.Lokacijas.FirstOrDefaultAsync(l => l.Id == id);

    public async Task<Lokacija> CreateAsync(Lokacija lokacija)
    {
        if (string.IsNullOrWhiteSpace(lokacija.Name))
            throw new ArgumentException("Naziv je obavezan.");

        _db.Lokacijas.Add(lokacija);
        await _db.SaveChangesAsync();
        return lokacija;
    }

    public async Task<bool> UpdateAsync(int id, Lokacija lokacija)
    {
        var existing = await _db.Lokacijas.FindAsync(id);
        if (existing == null) return false;

        existing.Name = lokacija.Name;
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existing = await _db.Lokacijas.FindAsync(id);
        if (existing == null) return false;

        _db.Lokacijas.Remove(existing);
        await _db.SaveChangesAsync();
        return true;
    }

    
}
