using Application.Common.Services.Location;
using Application.Common.Services.Location.Models;
using Domain.Services.Email.Models;
using System.Text.Json;

namespace Infrastructure.LocationManager
{
    public class NominatimAddressResolver : IAddressResolverService
    {
        private readonly HttpClient _httpClient;

        public NominatimAddressResolver(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AddressInfo?> ResolveAsync(ParsedAddress shippingAddress)
        {
            string formattedAddress = string.Concat(shippingAddress.Zip, " ", shippingAddress.Country, " ", shippingAddress.City, " ", shippingAddress.Street);

            if (string.Equals(shippingAddress.City, "Budapest", StringComparison.InvariantCultureIgnoreCase))
            {
                return new AddressInfo
                {
                    County = "Budapest",
                    City = shippingAddress.City,
                    Zip = shippingAddress.Zip
                };
            }

            var url =
                "https://nominatim.openstreetmap.org/search" +
                $"?q={Uri.EscapeDataString(formattedAddress)}" +
                "&format=json&addressdetails=1&limit=1";

            var request = new HttpRequestMessage(HttpMethod.Get, url);

            request.Headers.Add("User-Agent", "ProfilPlusz/1.0");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);

            var results = doc.RootElement;

            if (results.GetArrayLength() == 0)
            {
                return null;
            }

            var addressObj = results[0].GetProperty("address");

            return new AddressInfo
            {
                County = addressObj.TryGetProperty("county", out var county)
                    ? county.GetString()
                    : null,

                City = addressObj.TryGetProperty("city", out var city)
                    ? city.GetString()
                    : null,

                Zip = addressObj.TryGetProperty("postcode", out var zip)
                    ? zip.GetString()
                    : null
            };
        }
    }
}
