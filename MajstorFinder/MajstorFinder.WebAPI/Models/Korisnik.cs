using System;
using System.Collections.Generic;

namespace MajstorFinder.WebAPI.Models;

public partial class Korisnik
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual ICollection<Zahtjev> Zahtjevs { get; set; } = new List<Zahtjev>();
}
