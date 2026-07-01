using Application.Common.CQS.Queries;
using Application.Common.Extensions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Queries;

public record AddressListItemDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }
    public int Type { get; set; }
}

public record GetCustomerListDto
{
    public string? Id { get; init; }
    public string? Name { get; set; }
    public string? Number { get; set; }
    public string? Description { get; set; }

    public string? PhoneNumber { get; set; }
    public string? FaxNumber { get; set; }

    public string? EmailAddress { get; set; }
    public string? EmailAddressOrderConfirmation { get; set; }
    public string? EmailAddressInvoice { get; set; }
    public string? EmailAddressPurchaseOrder { get; set; }

    public string? Website { get; set; }
    public string? WhatsApp { get; set; }
    public string? LinkedIn { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? TwitterX { get; set; }
    public string? TikTok { get; set; }

    public string? ContactPersonName { get; set; }

    public string? TaxNumber { get; set; }
    public string? EuTaxNumber { get; set; }
    public string? BankAccountNumber { get; set; }

    public InvoiceType? InvoiceType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public int? PaymentDeadlineDays { get; set; }
    public Currency? Currency { get; set; }

    public string? City { get; set; }

    public List<AddressListItemDto>? Addresses { get; set; }

    public string? CustomerGroupId { get; set; }
    public string? CustomerGroupName { get; set; }
    public string? CustomerCategoryId { get; set; }
    public string? CustomerCategoryName { get; set; }

    public string? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
}

public class GetCustomerListProfile : Profile
{
    public GetCustomerListProfile()
    {
        CreateMap<Address, AddressListItemDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type));

        CreateMap<Customer, GetCustomerListDto>()
            .ForMember(
                dest => dest.CustomerGroupName,
                opt => opt.MapFrom(src => src.CustomerGroup != null ? src.CustomerGroup.Name : string.Empty))
            .ForMember(
                dest => dest.CustomerCategoryName,
                opt => opt.MapFrom(src => src.CustomerCategory != null ? src.CustomerCategory.Name : string.Empty))
            .ForMember(
                dest => dest.City,
                opt => opt.MapFrom(src => src.AddressList.FirstOrDefault() != null ? src.AddressList.FirstOrDefault()!.City : null))
            .ForMember(
                dest => dest.Addresses,
                opt => opt.MapFrom(src => src.AddressList));
    }
}

public class GetCustomerListResult
{
    public List<GetCustomerListDto>? Data { get; init; }
}

public class GetCustomerListRequest : IRequest<GetCustomerListResult>
{
    public bool IsDeleted { get; init; } = false;
}

public class GetCustomerListHandler : IRequestHandler<GetCustomerListRequest, GetCustomerListResult>
{
    private readonly IMapper _mapper;
    private readonly IQueryContext _context;

    public GetCustomerListHandler(IMapper mapper, IQueryContext context)
    {
        _mapper = mapper;
        _context = context;
    }

    public async Task<GetCustomerListResult> Handle(GetCustomerListRequest request, CancellationToken cancellationToken)
    {
        var query = _context
            .Customer
            .AsNoTracking()
            .ApplyIsDeletedFilter(request.IsDeleted)
            .Include(x => x.CustomerGroup)
            .Include(x => x.CustomerCategory)
            .Include(x => x.AddressList)
            .AsQueryable();

        var entities = await query.ToListAsync(cancellationToken);

        var dtos = _mapper.Map<List<GetCustomerListDto>>(entities);

        return new GetCustomerListResult
        {
            Data = dtos
        };
    }
}
