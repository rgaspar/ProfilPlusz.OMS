using Domain.Common;

namespace Domain.Entities;

public class ProductCustomer : BaseEntity
{
    public string? ProductId { get; set; }
    public Product? Product { get; set; }
    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }
}
