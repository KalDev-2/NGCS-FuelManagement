using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEXT_BMS.Models;

public partial class UserOrganization
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int OrganizationId { get; set; }

    public int ActionBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AssignedDate { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    [ForeignKey("ActionBy")]
    [InverseProperty("UserOrganizationActionByNavigations")]
    public virtual User ActionByNavigation { get; set; }

    [ForeignKey("OrganizationId")]
    [InverseProperty("UserOrganizations")]
    public virtual Organization Organization { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserOrganizationUsers")]
    public virtual User User { get; set; }
}
