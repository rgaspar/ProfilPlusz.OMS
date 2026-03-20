using Domain.Common.Location;

namespace Domain.Services.Location
{
    internal interface IAddressResolver
    {
        Task<AddressInfo> ResolveAsync(string address);
    }
}
