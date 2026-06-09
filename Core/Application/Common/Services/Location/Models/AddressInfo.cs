namespace Application.Common.Services.Location.Models
{
    public class AddressInfo
    {
        //public string? County { get; set; }       Nem kezeljük külön
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
    }
}
