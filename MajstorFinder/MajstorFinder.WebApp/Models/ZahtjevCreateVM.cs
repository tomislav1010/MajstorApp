using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class ZahtjevCreateVm
    {
        [Required]
        public int TvrtkaId { get; set; }

        [Required]
        public int VrstaRadaId { get; set; }

        // za sada jednostavno: koristimo jednog “demo” korisnika
        [Required]
        public int KorisnikId { get; set; } = 1;

        [Required(ErrorMessage = "Opis je obavezan.")]
        [StringLength(500)]
        public string Description { get; set; } = "";
    }
}
