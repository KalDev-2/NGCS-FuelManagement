using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class GasStation
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    [Required]
    [StringLength(100)]
    public string Location { get; set; }

    public int CityId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("GasStations")]
    public virtual City City { get; set; }

    [InverseProperty("GasStation")]
    public virtual ICollection<EntryAttempt> EntryAttempts { get; set; } = new List<EntryAttempt>();

    [InverseProperty("GasStation")]
    public virtual ICollection<FuelMeasure> FuelMeasures { get; set; } = new List<FuelMeasure>();

    [InverseProperty("GasStation")]
    public virtual ICollection<GasStationEntry> GasStationEntries { get; set; } = new List<GasStationEntry>();

    [InverseProperty("GasStation")]
    public virtual ICollection<GasStationFuelEntry> GasStationFuelEntries { get; set; } = new List<GasStationFuelEntry>();

    [InverseProperty("GasStation")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();
}
