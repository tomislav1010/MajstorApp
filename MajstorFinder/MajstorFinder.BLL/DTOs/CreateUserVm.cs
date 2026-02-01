using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.DTOs
{
   public class CreateUserDto
    {
        [Required]
        public string Username { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Required, MinLength(6)]
        public string Password { get; set; } = "";

        [Compare("Password")]
        public string ConfirmPassword { get; set; } = "";
        public bool IsAdmin { get; set; }
    }
}