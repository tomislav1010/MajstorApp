using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebAPI.DTOs
{
    
        public class LokacijaCreateDto
        {
            [Required] public string Name { get; set; } = null!;
        }
    
}
