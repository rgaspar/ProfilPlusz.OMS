using Application.Common.Repositories;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;

namespace Application.Features.CustomerManager.Commands;

public class CreateCustomerResult
{
    public Customer? Data { get; set; }
}

public class CreateCustomerContactDto
{
    public string? Name { get; set; }
    public string? JobTitle { get; set; }
    public string? PhoneNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string? EmailAddressOrderConfirmation { get; set; }
    public string? EmailAddressInvoice { get; set; }
    public string? EmailAddressPurchaseOrder { get; set; }
    public string? Description { get; set; }
}

public class CreateAddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    public AddressType Type { get; set; } = AddressType.Headquarters;
}

public class CreateCustomerRequest : IRequest<CreateCustomerResult>
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public string? WhatsApp { get; set; }
    public string? LinkedIn { get; set; }
    public string? Facebook { get; set; }
    public string? Instagram { get; set; }
    public string? TwitterX { get; set; }
    public string? TikTok { get; set; }

    public string? TaxNumber { get; set; }
    public string? EuTaxNumber { get; set; }
    public string? BankAccountNumber { get; set; }

    public InvoiceType? InvoiceType { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public int? PaymentDeadlineDays { get; set; }
    public Currency? Currency { get; set; }

    public string? CustomerGroupId { get; set; }
    public string? CustomerCategoryId { get; set; }

    public List<CreateAddressDto>? Addresses { get; set; }
    public List<CreateCustomerContactDto>? Contacts { get; set; }

    public string? CreatedById { get; init; }
}

public class CreateCustomerValidator : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty();

        RuleFor(x => x.CustomerGroupId)
            .NotEmpty();

        RuleFor(x => x.CustomerCategoryId)
            .NotEmpty();

        RuleForEach(x => x.Addresses)
            .ChildRules(addr =>
            {
                addr.RuleFor(x => x.Street).NotEmpty();
                addr.RuleFor(x => x.City).NotEmpty();
                addr.RuleFor(x => x.ZipCode).NotEmpty();
                addr.RuleFor(x => x.Country).NotEmpty();
            });
    }
}

public class CreateCustomerHandler : IRequestHandler<CreateCustomerRequest, CreateCustomerResult>
{
    private readonly ICommandRepository<Customer> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly NumberSequenceService _numberSequenceService;

    public CreateCustomerHandler(
        ICommandRepository<Customer> repository,
        IUnitOfWork unitOfWork,
        NumberSequenceService numberSequenceService)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _numberSequenceService = numberSequenceService;
    }

    public async Task<CreateCustomerResult> Handle(CreateCustomerRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Customer
        {
            CreatedById = request.CreatedById,

            Name = request.Name,
            Number = _numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
            Description = request.Description,

            WhatsApp = request.WhatsApp,
            LinkedIn = request.LinkedIn,
            Facebook = request.Facebook,
            Instagram = request.Instagram,
            TwitterX = request.TwitterX,
            TikTok = request.TikTok,

            TaxNumber = request.TaxNumber,
            EuTaxNumber = request.EuTaxNumber,
            BankAccountNumber = request.BankAccountNumber,

            InvoiceType = request.InvoiceType,
            PaymentMethod = request.PaymentMethod,
            PaymentDeadlineDays = request.PaymentDeadlineDays,
            Currency = request.Currency,

            CustomerGroupId = request.CustomerGroupId,
            CustomerCategoryId = request.CustomerCategoryId
        };

        if (request.Addresses != null && request.Addresses.Count > 0)
        {
            foreach (var addr in request.Addresses)
            {
                entity.AddressList.Add(new Address
                {
                    Street = addr.Street,
                    City = addr.City,
                    State = addr.State,
                    ZipCode = addr.ZipCode,
                    Country = addr.Country,
                    Type = addr.Type
                });
            }
        }

        if (request.Contacts != null && request.Contacts.Count > 0)
        {
            foreach (var contact in request.Contacts)
            {
                if (string.IsNullOrWhiteSpace(contact.Name)) continue;
                entity.CustomerContactList.Add(new CustomerContact
                {
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    PhoneNumber = contact.PhoneNumber,
                    EmailAddress = contact.EmailAddress,
                    EmailAddressOrderConfirmation = contact.EmailAddressOrderConfirmation,
                    EmailAddressInvoice = contact.EmailAddressInvoice,
                    EmailAddressPurchaseOrder = contact.EmailAddressPurchaseOrder,
                    Description = contact.Description
                });
            }
        }

        await _repository.CreateAsync(entity, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return new CreateCustomerResult
        {
            Data = entity
        };
    }
}