using System;
using System.Collections.Generic;

namespace MajstorFinder.DAL.Models;
public partial class VrstaRada
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int TvrtkaId { get; set; }

    public virtual Tvrtka Tvrtka { get; set; } = null!;

    public virtual ICollection<Zahtjev> Zahtjevs { get; set; } = new List<Zahtjev>();
}
