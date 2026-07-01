using Domain.Common;

namespace Domain.Entities;

public class PriceList : BaseEntity
{
    public string? ProductId { get; set; }
    public Product? Product { get; set; }

    public string? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public string? TaxId { get; set; }
    public Tax? Tax { get; set; }

    public double NetPrice { get; set; }
    public double? GrossPrice { get; set; }

    public decimal? QuantityDiscount { get; set; }
    public DateTime? DiscountFrom { get; set; }
    public DateTime? DiscountTo { get; set; }
}
