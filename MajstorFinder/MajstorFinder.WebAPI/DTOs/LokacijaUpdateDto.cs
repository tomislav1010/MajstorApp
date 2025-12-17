using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    public class LokacijaUpdateDto
    {
        [Required] public int Id { get; set; }
        [Required] public string Name { get; set; } = null!;
    }
}
