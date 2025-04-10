using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class GasStationEntry
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int VehicleId { get; set; }

    public int GasStationId { get; set; }

    public int FuelTypeId { get; set; }

    [Required]
    [StringLength(50)]
    public string PlateNumber { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EntryDate { get; set; }

    public double? Quantity { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("FuelTypeId")]
    [InverseProperty("GasStationEntries")]
    public virtual FuelType FuelType { get; set; }

    [ForeignKey("GasStationId")]
    [InverseProperty("GasStationEntries")]
    public virtual GasStation GasStation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("GasStationEntries")]
    public virtual User User { get; set; }

    [ForeignKey("VehicleId")]
    [InverseProperty("GasStationEntries")]
    public virtual Vehicle Vehicle { get; set; }
}
