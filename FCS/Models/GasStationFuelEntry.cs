using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class GasStationFuelEntry
{
    [Key]
    public int Id { get; set; }

    public int GasStationId { get; set; }

    public int FuelTypeId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Date { get; set; }

    public int ActionBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ActionBy")]
    [InverseProperty("GasStationFuelEntries")]
    public virtual User ActionByNavigation { get; set; }

    [ForeignKey("FuelTypeId")]
    [InverseProperty("GasStationFuelEntries")]
    public virtual FuelType FuelType { get; set; }

    [ForeignKey("GasStationId")]
    [InverseProperty("GasStationFuelEntries")]
    public virtual GasStation GasStation { get; set; }
}
