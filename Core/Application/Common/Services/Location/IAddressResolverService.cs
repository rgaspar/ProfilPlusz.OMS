using Application.Common.Services.Location.Models;
using Domain.Services.Email.Models;

namespace Application.Common.Services.Location
{
    public interface IAddressResolverService
    {
        Task<AddressInfo> ResolveAsync(ParsedAddress shippingAddress);
    }
}
