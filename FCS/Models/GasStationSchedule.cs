using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FCS.Models;

public partial class GasStationSchedule
{
    [Key]
    public int Id { get; set; }

    public int CityId { get; set; }

    public int GasStationId { get; set; }

    public int VehicleTypeId { get; set; }

    public int FuelTypeId { get; set; }

    public int PlateNumberCategoryId { get; set; }

    public DateOnly ScheduleDate { get; set; }

    public int ActionBy { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ActionBy")]
    [InverseProperty("GasStationSchedules")]
    public virtual User ActionByNavigation { get; set; }

    [ForeignKey("CityId")]
    [InverseProperty("GasStationSchedules")]
    public virtual City City { get; set; }

    [ForeignKey("FuelTypeId")]
    [InverseProperty("GasStationSchedules")]
    public virtual FuelType FuelType { get; set; }

    [ForeignKey("GasStationId")]
    [InverseProperty("GasStationSchedules")]
    public virtual GasStation GasStation { get; set; }

    [ForeignKey("PlateNumberCategoryId")]
    [InverseProperty("GasStationSchedules")]
    public virtual PlateNumberCategory PlateNumberCategory { get; set; }

    [ForeignKey("VehicleTypeId")]
    [InverseProperty("GasStationSchedules")]
    public virtual VehicleType VehicleType { get; set; }
}
