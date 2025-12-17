using System;
using System.Collections.Generic;

namespace MajstorFinder.WebAPI.Models;

public partial class Zahtjev
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public DateTime DateCreated { get; set; }

    public string Status { get; set; } = null!;

    public int KorisnikId { get; set; }

    public int TvrtkaId { get; set; }

    public int VrstaRadaId { get; set; }

    public virtual Korisnik Korisnik { get; set; } = null!;

    public virtual Tvrtka Tvrtka { get; set; } = null!;

    public virtual VrstaRada VrstaRada { get; set; } = null!;
}
