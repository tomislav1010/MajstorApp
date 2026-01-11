using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class TvrtkaVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [StringLength(100, ErrorMessage = "Naziv može imati max 100 znakova.")]
        public string Name { get; set; } = "";

        [StringLength(500, ErrorMessage = "Opis može imati max 500 znakova.")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Telefon može imati max 50 znakova.")]
        public string? Phone { get; set; }

        [EmailAddress(ErrorMessage = "Email nije ispravnog formata.")]
        [StringLength(100, ErrorMessage = "Email može imati max 100 znakova.")]
        public string? Email { get; set; }
    }
}
