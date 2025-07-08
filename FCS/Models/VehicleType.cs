using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class VehicleType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public int CodeId { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("CodeId")]
    [InverseProperty("VehicleTypes")]
    public virtual Code Code { get; set; }

    [InverseProperty("VehicleType")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();

    [InverseProperty("VehicleType")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
