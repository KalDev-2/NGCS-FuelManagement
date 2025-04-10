using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class Code
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("Code")]
    public virtual ICollection<VehicleType> VehicleTypes { get; set; } = new List<VehicleType>();

    [InverseProperty("Code")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
