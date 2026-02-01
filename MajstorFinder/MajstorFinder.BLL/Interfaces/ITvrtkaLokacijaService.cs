using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MajstorFinder.DAL.Models;

namespace MajstorFinder.BLL.Interfaces
{
    public interface ITvrtkaLokacijaService
    {
        Task<List<Lokacija>> GetLokacijeForTvrtkaAsync(int tvrtkaId);

        // sinkronizacija: stanje u bazi postaje točno ovo što pošalješ u lokacijaIds
        Task SetLokacijeForTvrtkaAsync(int tvrtkaId, IEnumerable<int> lokacijaIds);

        Task<List<TvrtkaLokacija>> GetByIdsAsync(int tvrtkaId, IEnumerable<int> lokacijasIds);

        Task<List<int>> GetLokacijeIdsForTvrtkaAsync(int tvrtkaId);

        Task AddAsync(int tvrtkaId, int lokacijaId);
        Task RemoveAsync(int tvrtkaId, int lokacijaId);
    }
}
