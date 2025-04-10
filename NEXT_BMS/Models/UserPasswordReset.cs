using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class UserPasswordReset
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Token { get; set; }

    [Required]
    [StringLength(12)]
    public string VerificationCode { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ExpiryDate { get; set; }

    public bool Validated { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserPasswordResets")]
    public virtual User User { get; set; }
}
