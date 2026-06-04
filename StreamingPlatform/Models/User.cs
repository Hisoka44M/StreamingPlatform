using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingPlatform.Models
{
    public class User
    {
        [Key]
        public int? UserID { get; set; }

        public string? Username { get; set; }

        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string? Country { get; set; }

            
    }
}
