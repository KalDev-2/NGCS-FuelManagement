using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NEXT_BMS.ViewModels;
public class PasswordResetViewModel
{

    public int uprId { get; set; }

    [DisplayName("Full Name")]
    public string FullName { get; set; }

    [DisplayName("Token")]
    public string Token { get; set; }

    [DisplayName("Photo")]
    public string Photo { get; set; }

    [DisplayName("Expiry Date")]
    public DateTime ExpiryDate { get; set; }

    [Required]
    [DisplayName("Reset Code")]
    [StringLength(6, ErrorMessage = "The {0} must be {2} characters long.", MinimumLength = 6)]
    public string VerificationCode { get; set; }

    [Required]
    [DisplayName("New Password")]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Confirm New Password")]
    [DataType(DataType.Password)]
    [CompareAttribute("Password", ErrorMessage = "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}
