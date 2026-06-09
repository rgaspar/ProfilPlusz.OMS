namespace Application.Common.DTOs.Location
{
    public class AddressInfoDto
    {
        public string? County { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
