using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.DTOs
{
    public class CreateZahtjevDto
    {
        public int TvrtkaId { get; set; }
        public int VrstaRadaId { get; set; }
        public string Description { get; set; } = " ";
    }
}
