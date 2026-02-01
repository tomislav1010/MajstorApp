using System.ComponentModel.DataAnnotations;

namespace MajstorFinder.WebApp.Models
{
    public class CreateUserVm
    {

            [Required, StringLength(50)]
            public string Username { get; set; } = "";

            [Required, EmailAddress, StringLength(100)]
            public string Email { get; set; } = "";

            [Required, StringLength(100, MinimumLength = 4)]
            [DataType(DataType.Password)]
            public string Password { get; set; } = "";

            [Required]
            [DataType(DataType.Password)]
            [Compare(nameof(Password), ErrorMessage = "Lozinke se ne podudaraju.")]
            public string ConfirmPassword { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
    }

