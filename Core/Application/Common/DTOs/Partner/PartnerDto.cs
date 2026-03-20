namespace Application.Common.DTOs.Partner
{
    public class PartnerDto
    {
        public string? CustomerCode { get; set; }
        public string? Category1 { get; set; }
        public string? Category2 { get; set; }
        public string? Name { get; set; }

        public string? TaxNumber { get; set; }
        public string? EuTaxNumber { get; set; }

        public string? ContactName { get; set; }
        public string? EmailOrderConfirmation { get; set; }
        public string? EmailInvoice { get; set; }
        public string? EmailPurchaseOrder { get; set; }

        public string? Phone { get; set; }

        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }

        public string? BankAccount { get; set; }

        public string? InvoiceType { get; set; }
        public string? PaymentMethod { get; set; }
        public string? PaymentDeadline { get; set; }
        public string? Currency { get; set; }
    }
}
