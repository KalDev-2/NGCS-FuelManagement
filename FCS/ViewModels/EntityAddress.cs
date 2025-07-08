namespace Portal;
public class EntityAddress
{

    public int Id { get; set; }
    public int EntityId { get; set; }
    public int AddressTypeId { get; set; }
    public string Value { get; set; }
   
    public string AddressTypeName { get; set; }
    public bool NameAsDisplay { get; set; }
    public int OrderNumber { get; set; }
    public bool HasOptions { get; set; }
    public string Icon { get; set; } = null!;
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }
}


