using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    public class TvrtkaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public string Phone { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;
    }
}

