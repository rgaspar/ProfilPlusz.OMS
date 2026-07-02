using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Commands;

public class UpdateCustomerResult
{
    public Customer? Data { get; set; }
}

public class UpdateCustomerContactDto
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

public class UpdateAddressDto
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public string? Country { get; set; }

    public AddressType Type { get; set; }
}

public class UpdateCustomerRequest : IRequest<UpdateCustomerResult>
{
    public string? Id { get; init; }

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

    public List<UpdateAddressDto>? Addresses { get; set; }
    public List<UpdateCustomerContactDto>? Contacts { get; set; }

    public string? UpdatedById { get; init; }
}

public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

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

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerRequest, UpdateCustomerResult>
{
    private readonly ICommandRepository<Customer> _repository;
    private readonly ICommandRepository<Address> _addressRepository;
    private readonly ICommandRepository<CustomerContact> _contactRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCustomerHandler(
        ICommandRepository<Customer> repository,
        ICommandRepository<Address> addressRepository,
        ICommandRepository<CustomerContact> contactRepository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _addressRepository = addressRepository;
        _contactRepository = contactRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UpdateCustomerResult> Handle(UpdateCustomerRequest request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetAsync(request.Id ?? string.Empty, cancellationToken);

        if (entity == null)
        {
            throw new Exception($"Customer not found: {request.Id}");
        }

        var existingAddresses = await _addressRepository.GetQuery()
            .Where(a => a.CustomerId == entity.Id)
            .ToListAsync(cancellationToken);
        foreach (var addr in existingAddresses)
            _addressRepository.Purge(addr);

        entity.UpdatedById = request.UpdatedById;

        entity.Name = request.Name;
        entity.Description = request.Description;

        entity.WhatsApp = request.WhatsApp;
        entity.LinkedIn = request.LinkedIn;
        entity.Facebook = request.Facebook;
        entity.Instagram = request.Instagram;
        entity.TwitterX = request.TwitterX;
        entity.TikTok = request.TikTok;

        entity.TaxNumber = request.TaxNumber;
        entity.EuTaxNumber = request.EuTaxNumber;
        entity.BankAccountNumber = request.BankAccountNumber;

        entity.InvoiceType = request.InvoiceType;
        entity.PaymentMethod = request.PaymentMethod;
        entity.PaymentDeadlineDays = request.PaymentDeadlineDays;
        entity.Currency = request.Currency;

        entity.CustomerGroupId = request.CustomerGroupId;
        entity.CustomerCategoryId = request.CustomerCategoryId;

        if (request.Addresses != null && request.Addresses.Count > 0)
        {
            foreach (var addr in request.Addresses)
            {
                await _addressRepository.CreateAsync(new Address
                {
                    CustomerId = entity.Id,
                    Street = addr.Street,
                    City = addr.City,
                    State = addr.State,
                    ZipCode = addr.ZipCode,
                    Country = addr.Country,
                    Type = addr.Type
                }, cancellationToken);
            }
        }

        var existingContacts = await _contactRepository.GetQuery()
            .Where(c => c.CustomerId == entity.Id)
            .ToListAsync(cancellationToken);
        foreach (var c in existingContacts)
            _contactRepository.Purge(c);

        if (request.Contacts != null && request.Contacts.Count > 0)
        {
            foreach (var contact in request.Contacts)
            {
                if (string.IsNullOrWhiteSpace(contact.Name)) continue;
                await _contactRepository.CreateAsync(new CustomerContact
                {
                    CustomerId = entity.Id,
                    Name = contact.Name,
                    JobTitle = contact.JobTitle,
                    PhoneNumber = contact.PhoneNumber,
                    EmailAddress = contact.EmailAddress,
                    EmailAddressOrderConfirmation = contact.EmailAddressOrderConfirmation,
                    EmailAddressInvoice = contact.EmailAddressInvoice,
                    EmailAddressPurchaseOrder = contact.EmailAddressPurchaseOrder,
                    Description = contact.Description
                }, cancellationToken);
            }
        }

        _repository.Update(entity);

        await _unitOfWork.SaveAsync(cancellationToken);

        return new UpdateCustomerResult
        {
            Data = entity
        };
    }
}