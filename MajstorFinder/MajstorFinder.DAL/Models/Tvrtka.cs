using System;
using System.Collections.Generic;

namespace MajstorFinder.DAL.Models;
public partial class Tvrtka
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual ICollection<VrstaRada> VrstaRadas { get; set; } = new List<VrstaRada>();

    public virtual ICollection<Zahtjev> Zahtjevs { get; set; } = new List<Zahtjev>();

   // public virtual ICollection<Lokacija> Lokacijas { get; set; } = new List<Lokacija>();
    public virtual ICollection<TvrtkaLokacija>TvrtkaLokacijas { get; set; }=new List<TvrtkaLokacija>();
}
