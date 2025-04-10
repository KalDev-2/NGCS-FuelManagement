using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class FuelType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(30)]
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("FuelType")]
    public virtual ICollection<FuelMeasure> FuelMeasures { get; set; } = new List<FuelMeasure>();

    [InverseProperty("FuelType")]
    public virtual ICollection<GasStationEntry> GasStationEntries { get; set; } = new List<GasStationEntry>();

    [InverseProperty("FuelType")]
    public virtual ICollection<GasStationFuelEntry> GasStationFuelEntries { get; set; } = new List<GasStationFuelEntry>();

    [InverseProperty("FuelType")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();

    [InverseProperty("FuelType")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
