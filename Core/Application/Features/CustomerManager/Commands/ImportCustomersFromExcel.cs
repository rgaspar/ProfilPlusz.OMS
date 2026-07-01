using Application.Common.Repositories;
using Application.Common.Services.ExcelImport;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.CustomerManager.Commands;

public class ImportCustomersFromExcelErrorDto
{
    public int RowNumber { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
}

public class ImportCustomersFromExcelResult
{
    public int SuccessCount { get; init; }
    public int ErrorCount { get; init; }
    public List<ImportCustomersFromExcelErrorDto> Errors { get; init; } = [];
}

public class ImportCustomersFromExcelRequest : IRequest<ImportCustomersFromExcelResult>
{
    public required Stream ExcelStream { get; init; }
    public string? CreatedById { get; init; }
}

public class ImportCustomersFromExcelHandler(
    ICommandRepository<Customer> repository,
    ICommandRepository<CustomerContact> contactRepository,
    IEntityDbSet db,
    IUnitOfWork unitOfWork,
    ExcelImportService excelImportService,
    IValidator<CreateCustomerRequest> validator,
    NumberSequenceService numberSequenceService
) : IRequestHandler<ImportCustomersFromExcelRequest, ImportCustomersFromExcelResult>
{
    public async Task<ImportCustomersFromExcelResult> Handle(
        ImportCustomersFromExcelRequest request,
        CancellationToken cancellationToken)
    {
        var customerGroups = await db.CustomerGroup
            .AsNoTracking()
            .Where(g => !g.IsDeleted)
            .ToDictionaryAsync(g => g.Name ?? string.Empty, g => g.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var customerCategories = await db.CustomerCategory
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .ToDictionaryAsync(c => c.Name ?? string.Empty, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var existingCustomersByName = await db.Customer
            .AsNoTracking()
            .Where(c => !c.IsDeleted)
            .ToDictionaryAsync(c => c.Name ?? string.Empty, c => c.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var allContacts = await contactRepository.GetQuery()
            .Where(c => !c.IsDeleted)
            .ToListAsync(cancellationToken);

        var contactsByCustomer = allContacts
            .Where(c => c.CustomerId != null)
            .GroupBy(c => c.CustomerId!)
            .ToDictionary(
                g => g.Key,
                g => g.ToDictionary(c => c.Name ?? string.Empty, c => c, StringComparer.OrdinalIgnoreCase));

        static string? RowGet(IDictionary<string, object> row, string key) =>
            row.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val?.ToString()) ? val.ToString() : null;

        var mapper = new CustomerExcelRowMapper(customerGroups, customerCategories);

        var result = await excelImportService.ImportAsync<CreateCustomerRequest>(
            request.ExcelStream,
            mapper,
            async (createRequest, row, _, ct) =>
            {
                var contactName = RowGet(row, "Kapcsolattartó neve");
                var contactPhone = RowGet(row, "Telefon");
                var contactEmailOC = RowGet(row, "E-mail - visszaigazolás");
                var contactEmailInv = RowGet(row, "E-mail - számlázás");
                var contactEmailPO = RowGet(row, "E-mail - beszerzés");

                var customerName = createRequest.Name?.Trim() ?? string.Empty;

                if (existingCustomersByName.TryGetValue(customerName, out var existingCustomerId))
                {
                    if (!string.IsNullOrWhiteSpace(contactName))
                    {
                        contactsByCustomer.TryGetValue(existingCustomerId, out var customerContacts);

                        if (customerContacts != null && customerContacts.TryGetValue(contactName, out var existingContact))
                        {
                            existingContact.PhoneNumber = contactPhone ?? existingContact.PhoneNumber;
                            existingContact.EmailAddressOrderConfirmation = contactEmailOC ?? existingContact.EmailAddressOrderConfirmation;
                            existingContact.EmailAddressInvoice = contactEmailInv ?? existingContact.EmailAddressInvoice;
                            existingContact.EmailAddressPurchaseOrder = contactEmailPO ?? existingContact.EmailAddressPurchaseOrder;
                            existingContact.UpdatedById = request.CreatedById;
                            contactRepository.Update(existingContact);
                        }
                        else
                        {
                            var newContact = new CustomerContact
                            {
                                CustomerId = existingCustomerId,
                                Name = contactName,
                                PhoneNumber = contactPhone,
                                EmailAddressOrderConfirmation = contactEmailOC,
                                EmailAddressInvoice = contactEmailInv,
                                EmailAddressPurchaseOrder = contactEmailPO,
                                CreatedById = request.CreatedById
                            };
                            await contactRepository.CreateAsync(newContact, ct);

                            if (!contactsByCustomer.ContainsKey(existingCustomerId))
                                contactsByCustomer[existingCustomerId] = new Dictionary<string, CustomerContact>(StringComparer.OrdinalIgnoreCase);
                            contactsByCustomer[existingCustomerId][contactName] = newContact;
                        }

                        await unitOfWork.SaveAsync(ct);
                    }

                    return null;
                }

                var validation = await validator.ValidateAsync(createRequest, ct);
                if (!validation.IsValid)
                    return string.Join("; ", validation.Errors.Select(e => e.ErrorMessage));

                var entity = new Customer
                {
                    CreatedById = request.CreatedById,
                    Number = numberSequenceService.GenerateNumber(nameof(Customer), "", "CST"),
                    Name = createRequest.Name,
                    Description = createRequest.Description,
                    EmailAddress = createRequest.EmailAddress,
                    TaxNumber = createRequest.TaxNumber,
                    EuTaxNumber = createRequest.EuTaxNumber,
                    BankAccountNumber = createRequest.BankAccountNumber,
                    InvoiceType = createRequest.InvoiceType,
                    PaymentMethod = createRequest.PaymentMethod,
                    PaymentDeadlineDays = createRequest.PaymentDeadlineDays,
                    Currency = createRequest.Currency,
                    CustomerGroupId = createRequest.CustomerGroupId,
                    CustomerCategoryId = createRequest.CustomerCategoryId
                };

                if (createRequest.Addresses != null && createRequest.Addresses.Count > 0)
                {
                    foreach (var addr in createRequest.Addresses)
                    {
                        entity.AddressList.Add(new Address
                        {
                            Street = addr.Street,
                            City = addr.City,
                            ZipCode = addr.ZipCode,
                            Country = addr.Country,
                            Type = addr.Type
                        });
                    }
                }

                if (!string.IsNullOrWhiteSpace(contactName))
                {
                    entity.CustomerContactList.Add(new CustomerContact
                    {
                        Name = contactName,
                        PhoneNumber = contactPhone,
                        EmailAddressOrderConfirmation = contactEmailOC,
                        EmailAddressInvoice = contactEmailInv,
                        EmailAddressPurchaseOrder = contactEmailPO,
                        CreatedById = request.CreatedById
                    });
                }

                await repository.CreateAsync(entity, ct);
                await unitOfWork.SaveAsync(ct);

                existingCustomersByName[customerName] = entity.Id;

                return null;
            },
            cancellationToken);

        return new ImportCustomersFromExcelResult
        {
            SuccessCount = result.SuccessCount,
            ErrorCount = result.ErrorCount,
            Errors = result.Errors
                .Select(e => new ImportCustomersFromExcelErrorDto { RowNumber = e.RowNumber, ErrorMessage = e.ErrorMessage })
                .ToList()
        };
    }
}

internal sealed class CustomerExcelRowMapper : IExcelRowMapper<CreateCustomerRequest>
{
    private readonly Dictionary<string, string> _groups;
    private readonly Dictionary<string, string> _categories;

    public string[] TemplateHeaders =>
    [
        "Ügyfél neve", "Ügyfélcsoport", "Ügyfélkategória", "E-mail",
        "Adószám", "Közösségi adószám", "Kapcsolattartó neve",
        "E-mail - visszaigazolás", "E-mail - számlázás", "E-mail - beszerzés",
        "Telefon", "Ország", "Irányítószám", "Város", "Utca, házszám",
        "Bankszámlaszám", "Számla típusa", "Fizetés módja", "Fizetési határidő", "Pénznem"
    ];

    public CustomerExcelRowMapper(Dictionary<string, string> groups, Dictionary<string, string> categories)
    {
        _groups = groups;
        _categories = categories;
    }

    public Task<CreateCustomerRequest> MapAsync(IDictionary<string, object> row, CancellationToken cancellationToken)
    {
        var groupName = Get(row, "Ügyfélcsoport");
        var categoryName = Get(row, "Ügyfélkategória");

        _groups.TryGetValue(groupName, out var groupId);
        _categories.TryGetValue(categoryName, out var categoryId);

        var country = GetOrNull(row, "Ország");
        var zipCode = GetOrNull(row, "Irányítószám");
        var city = GetOrNull(row, "Város");
        var street = GetOrNull(row, "Utca, házszám");

        var addresses = new List<CreateAddressDto>();
        if (!string.IsNullOrWhiteSpace(country) || !string.IsNullOrWhiteSpace(city))
        {
            addresses.Add(new CreateAddressDto
            {
                Country = country,
                ZipCode = zipCode,
                City = city,
                Street = street,
                Type = AddressType.Headquarters
            });
        }

        var request = new CreateCustomerRequest
        {
            Name = Get(row, "Ügyfél neve"),
            EmailAddress = Get(row, "E-mail"),
            CustomerGroupId = string.IsNullOrEmpty(groupId) ? null : groupId,
            CustomerCategoryId = string.IsNullOrEmpty(categoryId) ? null : categoryId,
            TaxNumber = GetOrNull(row, "Adószám"),
            EuTaxNumber = GetOrNull(row, "Közösségi adószám"),
            BankAccountNumber = GetOrNull(row, "Bankszámlaszám"),
            InvoiceType = ParseInvoiceType(Get(row, "Számla típusa")),
            PaymentMethod = ParsePaymentMethod(Get(row, "Fizetés módja")),
            PaymentDeadlineDays = GetInt(row, "Fizetési határidő"),
            Currency = ParseCurrency(Get(row, "Pénznem")),
            Addresses = addresses.Count > 0 ? addresses : null
        };

        return Task.FromResult(request);
    }

    private static string Get(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) ? val?.ToString() ?? string.Empty : string.Empty;

    private static string? GetOrNull(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) && !string.IsNullOrWhiteSpace(val?.ToString()) ? val.ToString() : null;

    private static int? GetInt(IDictionary<string, object> row, string key) =>
        row.TryGetValue(key, out var val) && int.TryParse(val?.ToString(), out var i) ? i : null;

    private static InvoiceType? ParseInvoiceType(string value) => value.ToLowerInvariant() switch
    {
        "elektronikus" => InvoiceType.Electronic,
        "papír" or "papir" => InvoiceType.Paper,
        _ => null
    };

    private static PaymentMethod? ParsePaymentMethod(string value) => value.ToLowerInvariant() switch
    {
        "átutalás" or "atutalas" => PaymentMethod.BankTransfer,
        "készpénz" or "keszpenz" => PaymentMethod.Cash,
        _ => null
    };

    private static Currency? ParseCurrency(string value) => value.ToUpperInvariant() switch
    {
        "HUF" => Currency.HUF,
        "EUR" => Currency.EUR,
        "USD" => Currency.USD,
        _ => null
    };
}
