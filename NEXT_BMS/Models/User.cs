using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(30)]
    public string MiddleName { get; set; }

    [StringLength(30)]
    public string LastName { get; set; }

    [Required]
    [StringLength(50)]
    public string PhoneNumber { get; set; }

    [Required]
    [StringLength(350)]
    public string Password { get; set; }

    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    [StringLength(100)]
    public string UserName { get; set; }

    public int GenderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    public int LanguageId { get; set; }

    public int? FailureCount { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLogin { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? BlockEndDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<EntryAttempt> EntryAttempts { get; set; } = new List<EntryAttempt>();

    [InverseProperty("ActionByNavigation")]
    public virtual ICollection<FuelMeasure> FuelMeasureActionByNavigations { get; set; } = new List<FuelMeasure>();

    [InverseProperty("ApprovedByNavigation")]
    public virtual ICollection<FuelMeasure> FuelMeasureApprovedByNavigations { get; set; } = new List<FuelMeasure>();

    [InverseProperty("User")]
    public virtual ICollection<GasStationEntry> GasStationEntries { get; set; } = new List<GasStationEntry>();

    [InverseProperty("ActionByNavigation")]
    public virtual ICollection<GasStationFuelEntry> GasStationFuelEntries { get; set; } = new List<GasStationFuelEntry>();

    [InverseProperty("ActionByNavigation")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();

    [ForeignKey("GenderId")]
    [InverseProperty("Users")]
    public virtual Gender Gender { get; set; }

    [ForeignKey("LanguageId")]
    [InverseProperty("Users")]
    public virtual Language Language { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<UserLogo> UserLogos { get; set; } = new List<UserLogo>();

    [InverseProperty("ActionByNavigation")]
    public virtual ICollection<UserOrganization> UserOrganizationActionByNavigations { get; set; } = new List<UserOrganization>();

    [InverseProperty("User")]
    public virtual ICollection<UserOrganization> UserOrganizationUsers { get; set; } = new List<UserOrganization>();

    [InverseProperty("User")]
    public virtual ICollection<UserPasswordReset> UserPasswordResets { get; set; } = new List<UserPasswordReset>();

    [InverseProperty("User")]
    public virtual ICollection<UserPassword> UserPasswords { get; set; } = new List<UserPassword>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
