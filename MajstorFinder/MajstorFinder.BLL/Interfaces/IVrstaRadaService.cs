using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MajstorFinder.DAL.Models;

namespace MajstorFinder.BLL.Interfaces;

public interface IVrstaRadaService
{
    Task<List<VrstaRada>> GetAllAsync(int? tvrtkaId, int page, int pageSize);
    Task<int> CountAsync(int? tvrtkaId);

    Task<VrstaRada?> GetByIdAsync(int id);

    Task<VrstaRada> CreateAsync(VrstaRada vrsta);
    Task<bool> UpdateAsync(int id, VrstaRada vrsta);
    Task<bool> DeleteAsync(int id);
}
