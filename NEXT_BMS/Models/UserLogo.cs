using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class UserLogo
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string FingerPrint { get; set; }

    [Required]
    [StringLength(250)]
    public string UserAgent { get; set; }

    [Required]
    [StringLength(50)]
    public string Platform { get; set; }

    [Required]
    [StringLength(50)]
    public string Browser { get; set; }

    [Required]
    [StringLength(50)]
    public string TimeZone { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime LogDate { get; set; }

    [Required]
    [StringLength(8)]
    public string VerificationCode { get; set; }

    public bool IsVerified { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserLogos")]
    public virtual User User { get; set; }
}
