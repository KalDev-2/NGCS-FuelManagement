using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class City
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("City")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();

    [InverseProperty("City")]
    public virtual ICollection<GasStation> GasStations { get; set; } = new List<GasStation>();

    [InverseProperty("City")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
