using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class CreateModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class LoginViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class UsersAndRolesModel
    {
        public IEnumerable<AppUser> Users { get; set; }

        public IEnumerable<AppRole> Roles { get; set; }

        public IEnumerable<Message> Messages { get; set; }
    }
}