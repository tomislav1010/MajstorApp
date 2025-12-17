using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    public class VrstaRadaCreateDto
    {
        [Required] public string Name { get; set; } = null!;
        [Required] public int TvrtkaId { get; set; }
    }
}
