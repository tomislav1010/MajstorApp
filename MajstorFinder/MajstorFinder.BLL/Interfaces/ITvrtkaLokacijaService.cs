using MajstorFinder.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.Interfaces
{
    public interface ITvrtkaLokacijaService
    {
      
            Task<List<Lokacija>> GetLokacijeForTvrtkaAsync(int tvrtkaId);
            Task SetLokacijeForTvrtkaAsync(int tvrtkaId, IEnumerable<int> lokacijaIds);
        
    }
}
