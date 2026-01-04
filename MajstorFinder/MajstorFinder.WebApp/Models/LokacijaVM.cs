using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class LokacijaVm
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
    }
}
