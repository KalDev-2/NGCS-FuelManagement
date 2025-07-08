using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class EntryAttempt
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public int GasStationId { get; set; }

    [Required]
    [StringLength(50)]
    public string PlateNumber { get; set; }

    public DateOnly AttemptedDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("GasStationId")]
    [InverseProperty("EntryAttempts")]
    public virtual GasStation GasStation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EntryAttempts")]
    public virtual User User { get; set; }

    [ForeignKey("VehicleId")]
    [InverseProperty("EntryAttempts")]
    public virtual Vehicle Vehicle { get; set; }
}
