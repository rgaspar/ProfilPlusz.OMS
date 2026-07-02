using Application.Common.Repositories;
using Application.Features.NumberSequenceManager;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.SeedManager.Demos;

public class CustomerSeeder
{
    private readonly ICommandRepository<Customer> _customerRepository;
    private readonly ICommandRepository<CustomerGroup> _groupRepository;
    private readonly ICommandRepository<CustomerCategory> _categoryRepository;
    private readonly NumberSequenceService _numberSequenceService;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerSeeder(
        ICommandRepository<Customer> customerRepository,
        ICommandRepository<CustomerGroup> groupRepository,
        ICommandRepository<CustomerCategory> categoryRepository,
        NumberSequenceService numberSequenceService,
        IUnitOfWork unitOfWork
    )
    {
        _customerRepository = customerRepository;
        _groupRepository = groupRepository;
        _categoryRepository = categoryRepository;
        _numberSequenceService = numberSequenceService;
        _unitOfWork = unitOfWork;
    }

    public async Task GenerateDataAsync()
    {
        var groups = (await _groupRepository.GetQuery().ToListAsync()).Select(x => x.Id).ToArray();
        var categories = (await _categoryRepository.GetQuery().ToListAsync()).Select(x => x.Id).ToArray();

        var cities = new[] { "New York", "Los Angeles", "San Francisco", "Chicago" };
        var streets = new[] { "Main St", "Broadway", "Market St", "Elm St" };
        var states = new[] { "Bács-Kiskun vármegye", "Baranya vármegye", "Békés vármegye", "Borsod-Abaúj-Zemplén vármegye", "Csongrád-Csanád vármegye", "Fejér vármegye", "Győr-Moson-Sopron vármegye", "Hajdú-Bihar vármegye", "Heves vármegye", "Jász-Nagykun-Szolnok vármegye", "Komárom-Esztergom vármegye", "Nógrád vármegye", "Pest vármegye", "Somogy vármegye", "Szabolcs-Szatmár-Bereg vármegye", "Tolna vármegye", "Vas vármegye", "Veszprém vármegye", "Zala vármegye", "Budapest" };
        var zipCodes = new[] { "10001", "90001", "94101", "60601" };
        var phoneNumbers = new[] { "555-1234", "555-5678", "555-8765", "555-4321" };
        var emailDomains = new[] { "example.com", "demo.com", "test.com", "sample.com" };
        var currencies = new[] { Currency.USD, Currency.EUR };

        var random = new Random();

        var customers = new List<Customer>
        {
            /*new Customer { Name = "Citadel LLC" },
            new Customer { Name = "Ironclad LLC" },
            new Customer { Name = "Armada LLC" },
            new Customer { Name = "Shield LLC" },
            new Customer { Name = "Alpha LLC" },
            new Customer { Name = "Capitol LLC" },
            new Customer { Name = "Federal LLC" },
            new Customer { Name = "Statewide LLC" },
            new Customer { Name = "Harmony LLC" },
            new Customer { Name = "Hope LLC" },
            new Customer { Name = "Unity LLC" },
            new Customer { Name = "Prosperity LLC" },
            new Customer { Name = "Global LLC" },
            new Customer { Name = "Sunset LLC" },
            new Customer { Name = "Luxe LLC" },
            new Customer { Name = "Serenity LLC" },
            new Customer { Name = "Oasis LLC" },
            new Customer { Name = "Grandeur LLC" },
            new Customer { Name = "Bright LLC" },*/
            new Customer { Name = "Stellar LLC" }
        };

        foreach (var customer in customers)
        {
            var baseName = customer.Name?.Split(' ')[0].ToLower();

            customer.Number = _numberSequenceService.GenerateNumber(nameof(Customer), "", "CST");

            customer.CustomerGroupId = GetRandomValue(groups, random);
            customer.CustomerCategoryId = GetRandomValue(categories, random);

            customer.EmailAddress = $"{baseName}@{GetRandomString(emailDomains, random)}";

            customer.TaxNumber = $"{random.Next(10000000, 99999999)}-2-42";
            customer.EuTaxNumber = $"EU{random.Next(10000000, 99999999)}";

            customer.BankAccountNumber = $"{random.Next(10000000, 99999999)}-{random.Next(10000000, 99999999)}";

            customer.InvoiceType = InvoiceType.Paper;
            customer.PaymentMethod = GetRandomValue(new[]
            {
                PaymentMethod.BankTransfer,
                PaymentMethod.Cash
            }, random);

            customer.PaymentDeadlineDays = GetRandomValue(new[] { 8, 15, 30 }, random);

            customer.Currency = GetRandomValue(currencies, random);

            // cím hozzáadása
            customer.AddressList.Add(new Address
            {
                Street = GetRandomString(streets, random),
                City = GetRandomString(cities, random),
                State = GetRandomString(states, random),
                ZipCode = GetRandomString(zipCodes, random),
                Country = "USA",
                Type = AddressType.Headquarters
            });

            await _customerRepository.CreateAsync(customer);
        }

        await _unitOfWork.SaveAsync();
    }

    private static T GetRandomValue<T>(T[] array, Random random)
    {
        return array[random.Next(array.Length)];
    }

    private static string GetRandomString(string[] array, Random random)
    {
        return array[random.Next(array.Length)];
    }
}