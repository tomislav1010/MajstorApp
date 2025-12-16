using System;
using System.Collections.Generic;

namespace MajstorFinder.WebAPI.Models;

public partial class Lokacija
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Tvrtka> Tvrtkas { get; set; } = new List<Tvrtka>();
}
