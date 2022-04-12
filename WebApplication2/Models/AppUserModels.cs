using System;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebApplication2.Models
{
    public class AppUser : IdentityUser
    {
        public DateTime lastLogin { get; set; }   

        public DateTime registerDate { get; set; }
    }
}