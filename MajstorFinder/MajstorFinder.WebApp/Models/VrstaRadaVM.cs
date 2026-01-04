using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class VrstaRadaVm
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [Required]
        public int TvrtkaId { get; set; }

        // samo za prikaz u listi
        public string? TvrtkaName { get; set; }
    }
}
