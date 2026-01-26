using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MajstorFinder.DAL.Models;

namespace MajstorFinder.BLL.Interfaces;

public interface ILokacijaService
{
    Task<List<Lokacija>> GetAllAsync(string? q, int page, int pageSize);
    Task<int> CountAsync(string? q);

    Task<Lokacija?> GetByIdAsync(int id);

    Task<Lokacija> CreateAsync(Lokacija lokacija);
    Task<bool> UpdateAsync(int id, Lokacija lokacija);
    Task<bool> DeleteAsync(int id);
}
