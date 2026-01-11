using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class LokacijaVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan.")]
        [StringLength(100, ErrorMessage = "Naziv može imati max 100 znakova.")]
        public string Name { get; set; } = "";
    }
}
