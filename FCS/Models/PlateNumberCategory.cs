using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class PlateNumberCategory
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("PlateNumberCategory")]
    public virtual ICollection<GasStationSchedule> GasStationSchedules { get; set; } = new List<GasStationSchedule>();
}
