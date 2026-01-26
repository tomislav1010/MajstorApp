using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.Interfaces
{
    public interface ITvrtkaService
    {
        Task<List<Tvrtka>> GetAllAsync(string? q, int page, int pageSize);
        Task<int> CountAsync(string? q);

        Task<Tvrtka?> GetByIdAsync(int id);

        Task<Tvrtka> CreateAsync(Tvrtka tvrtka);
        Task<bool> UpdateAsync(int id, Tvrtka tvrtka);
        Task<bool> DeleteAsync(int id);
        Task<List<Lokacija>> GetLokacijeAsync(int tvrtkaId); 
    }
}


