using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class Menu
{
    [Key]
    public int Id { get; set; }

    public int MenuCategoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string Title { get; set; }

    [Required]
    [StringLength(50)]
    public string Controller { get; set; }

    [Required]
    [StringLength(50)]
    public string Action { get; set; }

    public bool IsMenu { get; set; }

    public bool IsTraceable { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("MenuCategoryId")]
    [InverseProperty("Menus")]
    public virtual MenuCategory MenuCategory { get; set; }

    [InverseProperty("Menu")]
    public virtual ICollection<RolesMenu> RolesMenus { get; set; } = new List<RolesMenu>();
}
