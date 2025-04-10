using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NEXT_BMS.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DisplayName("Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [DisplayName("New Password")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Confirm New Password")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.CompareAttribute("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}