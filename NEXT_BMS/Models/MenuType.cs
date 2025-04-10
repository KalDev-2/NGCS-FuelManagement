using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class MenuType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [StringLength(50)]
    public string PrimaryController { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [InverseProperty("MenuType")]
    public virtual ICollection<MenuCategory> MenuCategories { get; set; } = new List<MenuCategory>();
}
