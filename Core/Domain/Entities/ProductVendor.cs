using Domain.Common;

namespace Domain.Entities;

public class ProductVendor : BaseEntity
{
    public string? ProductId { get; set; }
    public Product? Product { get; set; }
    public string? VendorId { get; set; }
    public Vendor? Vendor { get; set; }
}
