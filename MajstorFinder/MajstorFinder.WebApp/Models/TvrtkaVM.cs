using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class TvrtkaVm
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? Phone { get; set; }

        [EmailAddress, MaxLength(100)]
        public string? Email { get; set; }
    }
}
