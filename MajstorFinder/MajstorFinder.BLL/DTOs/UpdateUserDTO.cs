using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MajstorFinder.BLL.DTOs
{
    public class UpdateUserDto
    {
        [Required, StringLength(50)]
        public string Username { get; set; } = "";

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [StringLength(30)]
        public string? Phone { get; set; }
    }
}
