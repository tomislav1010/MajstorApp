using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MajstorFinder.BLL.DTOs;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.Interfaces
{
    public interface IZahtjevService
    {
        Task<List<Zahtjev>> GetAllAsync();
        Task<List<Zahtjev>> GetByKorisnikAsync(int korisnikId);
        Task<Zahtjev> GetByIdAsync(int id);

        Task<int> CreateAsync(int korisnikid, CreateZahtijevDto dto);
        Task UpdateAsync(int id, string status);
        Task DeleteAsync(int id);
    }
}
