using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class Vehicle
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public int CityId { get; set; }

    public int VehicleTypeId { get; set; }

    public int FuelTypeId { get; set; }

    public int AreaId { get; set; }

    public int CodeId { get; set; }

    public int VehicleStatusId { get; set; }

    [Required]
    [StringLength(100)]
    public string PlateNumber { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("AreaId")]
    [InverseProperty("Vehicles")]
    public virtual Area Area { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("Vehicles")]
    public virtual City City { get; set; }

    [ForeignKey("CodeId")]
    [InverseProperty("Vehicles")]
    public virtual Code Code { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<EntryAttempt> EntryAttempts { get; set; } = new List<EntryAttempt>();

    [ForeignKey("FuelTypeId")]
    [InverseProperty("Vehicles")]
    public virtual FuelType FuelType { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<GasStationEntry> GasStationEntries { get; set; } = new List<GasStationEntry>();

    [ForeignKey("VehicleStatusId")]
    [InverseProperty("Vehicles")]
    public virtual VehicleStatus VehicleStatus { get; set; }

    [ForeignKey("VehicleTypeId")]
    [InverseProperty("Vehicles")]
    public virtual VehicleType VehicleType { get; set; }
}
