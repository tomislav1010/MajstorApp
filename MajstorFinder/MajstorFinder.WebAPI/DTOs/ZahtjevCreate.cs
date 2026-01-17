using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    public class ZahtjevCreateDto
    {
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = "";

        [Required]
        public int KorisnikId { get; set; }

        [Required]
        public int TvrtkaId { get; set; }

        [Required]
        public int VrstaRadaId { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Poslano";
    }
}

