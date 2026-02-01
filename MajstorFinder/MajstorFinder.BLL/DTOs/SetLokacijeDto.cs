using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.DTOs
{
    public class SetLokacijeDto
    {
        public int TvrtkaId { get; set; }
        public List<int> Lokacije { get; set; } = new();
    }
}
