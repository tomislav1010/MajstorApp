using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    public class VrstaRadaUpdateDto
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
        [Required] public int TvrtkaId { get; set; }
    }
}
