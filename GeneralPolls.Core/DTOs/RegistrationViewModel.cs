using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPolls.Core.DTOs
{
    public class RegistrationViewModel
    {
        [Required(ErrorMessage = " First Name Required")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name Required")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email Required")]
        public string Email { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Length must be at least 8 characters", MinimumLength = 8)]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage ="Confirm Password not the same as Password")]
        public string ConfirmPassword { get; set; }
    }
}
