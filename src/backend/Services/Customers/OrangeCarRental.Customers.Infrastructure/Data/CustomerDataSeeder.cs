using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Aggregates;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;

/// <summary>
/// Seeds the Customers database with sample German customers for development and testing.
/// Creates realistic customer profiles with German names, addresses, and phone numbers.
/// </summary>
public class CustomerDataSeeder
{
    private readonly CustomersDbContext _context;
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerDataSeeder> _logger;

    public CustomerDataSeeder(
        CustomersDbContext context,
        ICustomerRepository repository,
        ILogger<CustomerDataSeeder> logger)
    {
        _context = context;
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Seeds the database with sample customers if no customers exist.
    /// </summary>
    public async Task SeedAsync()
    {
        // Check if data already exists
        var existingCount = await _context.Customers.CountAsync();
        if (existingCount > 0)
        {
            _logger.LogInformation("Customers database already contains {Count} customers. Skipping seed.", existingCount);
            return;
        }

        _logger.LogInformation("Seeding Customers database with sample German customers...");

        var customers = CreateSampleCustomers();

        foreach (var customer in customers)
        {
            await _repository.AddAsync(customer);
        }

        await _repository.SaveChangesAsync();

        _logger.LogInformation("Successfully seeded {Count} customers.", customers.Count);
    }

    private static List<Customer> CreateSampleCustomers()
    {
        var customers = new List<Customer>();

        // Customer 1: Maximilian Schmidt - Young professional from Berlin
        customers.Add(Customer.Register(
            firstName: "Maximilian",
            lastName: "Schmidt",
            email: Email.Of("max.schmidt@email.de"),
            phoneNumber: PhoneNumber.Of("+49 30 12345678"),
            dateOfBirth: new DateOnly(1995, 3, 15),
            address: Address.Of("Hauptstraße 42", "Berlin", "10115", "Germany"),
            driversLicense: DriversLicense.Of("B123456789DE", "Germany", new DateOnly(2015, 6, 1), new DateOnly(2030, 6, 1))
        ));

        // Customer 2: Anna Müller - Mid-career from Munich
        customers.Add(Customer.Register(
            firstName: "Anna",
            lastName: "Müller",
            email: Email.Of("anna.mueller@mail.de"),
            phoneNumber: PhoneNumber.Of("+49 89 98765432"),
            dateOfBirth: new DateOnly(1988, 7, 22),
            address: Address.Of("Marienplatz 5", "München", "80331", "Germany"),
            driversLicense: DriversLicense.Of("B987654321DE", "Germany", new DateOnly(2008, 9, 15), new DateOnly(2028, 9, 15))
        ));

        // Customer 3: Lukas Weber - Recent graduate from Hamburg
        customers.Add(Customer.Register(
            firstName: "Lukas",
            lastName: "Weber",
            email: Email.Of("l.weber@webmail.de"),
            phoneNumber: PhoneNumber.Of("+49 40 55512345"),
            dateOfBirth: new DateOnly(1999, 11, 8),
            address: Address.Of("Reeperbahn 150", "Hamburg", "20359", "Germany"),
            driversLicense: DriversLicense.Of("B555123456DE", "Germany", new DateOnly(2019, 3, 10), new DateOnly(2034, 3, 10))
        ));

        // Customer 4: Sophie Wagner - Senior professional from Frankfurt
        customers.Add(Customer.Register(
            firstName: "Sophie",
            lastName: "Wagner",
            email: Email.Of("sophie.wagner@gmx.de"),
            phoneNumber: PhoneNumber.Of("+49 69 77788899"),
            dateOfBirth: new DateOnly(1980, 4, 30),
            address: Address.Of("Zeil 80", "Frankfurt am Main", "60313", "Germany"),
            driversLicense: DriversLicense.Of("B777888999DE", "Germany", new DateOnly(2010, 8, 20), new DateOnly(2030, 8, 20))
        ));

        // Customer 5: Jonas Fischer - Young driver from Cologne
        customers.Add(Customer.Register(
            firstName: "Jonas",
            lastName: "Fischer",
            email: Email.Of("jonas.fischer@outlook.de"),
            phoneNumber: PhoneNumber.Of("+49 221 33344455"),
            dateOfBirth: new DateOnly(2001, 9, 12),
            address: Address.Of("Hohe Straße 25", "Köln", "50667", "Germany"),
            driversLicense: DriversLicense.Of("B333444555DE", "Germany", new DateOnly(2020, 2, 28), new DateOnly(2035, 2, 28))
        ));

        // Customer 6: Emma Becker - Experienced driver from Stuttgart
        customers.Add(Customer.Register(
            firstName: "Emma",
            lastName: "Becker",
            email: Email.Of("emma.becker@t-online.de"),
            phoneNumber: PhoneNumber.Of("+49 711 22211100"),
            dateOfBirth: new DateOnly(1985, 1, 18),
            address: Address.Of("Königstraße 15", "Stuttgart", "70173", "Germany"),
            driversLicense: DriversLicense.Of("B222111000DE", "Germany", new DateOnly(2005, 5, 12), new DateOnly(2026, 5, 12))
        ));

        // Customer 7: Felix Hoffmann - Mid-age from Düsseldorf
        customers.Add(Customer.Register(
            firstName: "Felix",
            lastName: "Hoffmann",
            email: Email.Of("f.hoffmann@web.de"),
            phoneNumber: PhoneNumber.Of("+49 211 66677788"),
            dateOfBirth: new DateOnly(1990, 12, 5),
            address: Address.Of("Königsallee 60", "Düsseldorf", "40212", "Germany"),
            driversLicense: DriversLicense.Of("B666777888DE", "Germany", new DateOnly(2012, 1, 15), new DateOnly(2027, 1, 15))
        ));

        // Customer 8: Mia Schäfer - Young professional from Leipzig
        customers.Add(Customer.Register(
            firstName: "Mia",
            lastName: "Schäfer",
            email: Email.Of("mia.schaefer@email.de"),
            phoneNumber: PhoneNumber.Of("+49 341 44455566"),
            dateOfBirth: new DateOnly(1996, 6, 27),
            address: Address.Of("Petersstraße 30", "Leipzig", "04109", "Germany"),
            driversLicense: DriversLicense.Of("B444555666DE", "Germany", new DateOnly(2016, 8, 22), new DateOnly(2031, 8, 22))
        ));

        // Customer 9: Leon Meyer - Senior citizen from Bremen
        customers.Add(Customer.Register(
            firstName: "Leon",
            lastName: "Meyer",
            email: Email.Of("leon.meyer@posteo.de"),
            phoneNumber: PhoneNumber.Of("+49 421 11122233"),
            dateOfBirth: new DateOnly(1955, 2, 14),
            address: Address.Of("Böttcherstraße 8", "Bremen", "28195", "Germany"),
            driversLicense: DriversLicense.Of("B111222333DE", "Germany", new DateOnly(1975, 6, 10), new DateOnly(2026, 6, 10))
        ));

        // Customer 10: Lena Koch - Mid-career from Hannover
        customers.Add(Customer.Register(
            firstName: "Lena",
            lastName: "Koch",
            email: Email.Of("lena.koch@yahoo.de"),
            phoneNumber: PhoneNumber.Of("+49 511 88899900"),
            dateOfBirth: new DateOnly(1987, 10, 3),
            address: Address.Of("Kröpcke 10", "Hannover", "30159", "Germany"),
            driversLicense: DriversLicense.Of("B888999000DE", "Germany", new DateOnly(2007, 11, 5), new DateOnly(2027, 11, 5))
        ));

        // Customer 11: Paul Richter - Young entrepreneur from Dresden
        customers.Add(Customer.Register(
            firstName: "Paul",
            lastName: "Richter",
            email: Email.Of("paul.richter@gmail.com"),
            phoneNumber: PhoneNumber.Of("+49 351 55566677"),
            dateOfBirth: new DateOnly(1992, 8, 19),
            address: Address.Of("Prager Straße 20", "Dresden", "01069", "Germany"),
            driversLicense: DriversLicense.Of("B555666777DE", "Germany", new DateOnly(2013, 4, 18), new DateOnly(2028, 4, 18))
        ));

        // Customer 12: Laura Zimmermann - Student from Freiburg
        customers.Add(Customer.Register(
            firstName: "Laura",
            lastName: "Zimmermann",
            email: Email.Of("laura.zimmer@student.de"),
            phoneNumber: PhoneNumber.Of("+49 761 33322211"),
            dateOfBirth: new DateOnly(2002, 5, 25),
            address: Address.Of("Kaiser-Joseph-Straße 200", "Freiburg", "79098", "Germany"),
            driversLicense: DriversLicense.Of("B333222111DE", "Germany", new DateOnly(2021, 7, 12), new DateOnly(2036, 7, 12))
        ));

        // Customer 13: David Klein - Suspended customer (late payment issue)
        var suspendedCustomer = Customer.Register(
            firstName: "David",
            lastName: "Klein",
            email: Email.Of("david.klein@example.de"),
            phoneNumber: PhoneNumber.Of("+49 228 99988877"),
            dateOfBirth: new DateOnly(1983, 3, 8),
            address: Address.Of("Münsterplatz 12", "Bonn", "53111", "Germany"),
            driversLicense: DriversLicense.Of("B999888777DE", "Germany", new DateOnly(2003, 12, 1), new DateOnly(2028, 12, 1))
        );
        suspendedCustomer.Suspend("Pending payment verification");
        customers.Add(suspendedCustomer);

        // Customer 14: Sarah Lang - Older driver from Nuremberg
        customers.Add(Customer.Register(
            firstName: "Sarah",
            lastName: "Lang",
            email: Email.Of("sarah.lang@freenet.de"),
            phoneNumber: PhoneNumber.Of("+49 911 12312312"),
            dateOfBirth: new DateOnly(1970, 11, 20),
            address: Address.Of("Kaiserstraße 50", "Nürnberg", "90403", "Germany"),
            driversLicense: DriversLicense.Of("B123123123DE", "Germany", new DateOnly(1990, 3, 15), new DateOnly(2027, 3, 15))
        ));

        // Customer 15: Tim Schwarz - Recent driver
        customers.Add(Customer.Register(
            firstName: "Tim",
            lastName: "Schwarz",
            email: Email.Of("tim.schwarz@mailbox.org"),
            phoneNumber: PhoneNumber.Of("+49 201 45645645"),
            dateOfBirth: new DateOnly(1998, 7, 7),
            address: Address.Of("Limbecker Platz 1", "Essen", "45127", "Germany"),
            driversLicense: DriversLicense.Of("B456456456DE", "Germany", new DateOnly(2018, 10, 20), new DateOnly(2033, 10, 20))
        ));

        return customers;
    }
}
