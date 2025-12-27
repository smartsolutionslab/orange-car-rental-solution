using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;

/// <summary>
///     Seeds the Customers database with sample German customers for development and testing.
///     Creates realistic customer profiles with German names, addresses, and phone numbers.
/// </summary>
public class CustomerDataSeeder(
    CustomersDbContext context,
    ILogger<CustomerDataSeeder> logger)
{
    /// <summary>
    ///     Seeds the database with sample customers if no customers exist.
    /// </summary>
    public async Task SeedAsync()
    {
        // Check if data already exists
        var existingCount = await context.Customers.CountAsync();
        if (existingCount > 0)
        {
            logger.LogInformation("Customers database already contains {Count} customers. Skipping seed.", existingCount);
            return;
        }

        logger.LogInformation("Seeding Customers database with sample German customers...");

        var customers = CreateSampleCustomers();
        await context.Customers.AddRangeAsync(customers);
        await context.SaveChangesAsync();

        logger.LogInformation("Successfully seeded {Count} customers.", customers.Count);
    }

    private static List<Customer> CreateSampleCustomers()
    {
        var customers = new List<Customer>();

        // Customer 1: Maximilian Schmidt - Young professional from Berlin
        customers.Add(CreateCustomer(
            CustomerName.Of("Maximilian", "Schmidt"),
            Email.From("max.schmidt@email.de"),
            PhoneNumber.From("+49 30 12345678"),
            BirthDate.Of(1995, 3, 15),
            Address.Of("Hauptstraße 42", "Berlin", "10115"),
            DriversLicense.Of("B123456789DE", "Germany", new DateOnly(2015, 6, 1), new DateOnly(2030, 6, 1))
        ));

        // Customer 2: Anna Müller - Mid-career from Munich
        customers.Add(CreateCustomer(
            CustomerName.Of("Anna", "Müller"),
            Email.From("anna.mueller@mail.de"),
            PhoneNumber.From("+49 89 98765432"),
            BirthDate.Of(1988, 7, 22),
            Address.Of("Marienplatz 5", "München", "80331"),
            DriversLicense.Of("B987654321DE", "Germany", new DateOnly(2008, 9, 15), new DateOnly(2028, 9, 15))
        ));

        // Customer 3: Lukas Weber - Recent graduate from Hamburg
        customers.Add(CreateCustomer(
            CustomerName.Of("Lukas", "Weber"),
            Email.From("l.weber@webmail.de"),
            PhoneNumber.From("+49 40 55512345"),
            BirthDate.Of(1999, 11, 8),
            Address.Of("Reeperbahn 150", "Hamburg", "20359"),
            DriversLicense.Of("B555123456DE", "Germany", new DateOnly(2019, 3, 10), new DateOnly(2034, 3, 10))
        ));

        // Customer 4: Sophie Wagner - Senior professional from Frankfurt
        customers.Add(CreateCustomer(
            CustomerName.Of("Sophie", "Wagner"),
            Email.From("sophie.wagner@gmx.de"),
            PhoneNumber.From("+49 69 77788899"),
            BirthDate.Of(1980, 4, 30),
            Address.Of("Zeil 80", "Frankfurt am Main", "60313"),
            DriversLicense.Of("B777888999DE", "Germany", new DateOnly(2010, 8, 20), new DateOnly(2030, 8, 20))
        ));

        // Customer 5: Jonas Fischer - Young driver from Cologne
        customers.Add(CreateCustomer(
            CustomerName.Of("Jonas", "Fischer"),
            Email.From("jonas.fischer@outlook.de"),
            PhoneNumber.From("+49 221 33344455"),
            BirthDate.Of(2001, 9, 12),
            Address.Of("Hohe Straße 25", "Köln", "50667"),
            DriversLicense.Of("B333444555DE", "Germany", new DateOnly(2020, 2, 28), new DateOnly(2035, 2, 28))
        ));

        // Customer 6: Emma Becker - Experienced driver from Stuttgart
        customers.Add(CreateCustomer(
            CustomerName.Of("Emma", "Becker"),
            Email.From("emma.becker@t-online.de"),
            PhoneNumber.From("+49 711 22211100"),
            BirthDate.Of(1985, 1, 18),
            Address.Of("Königstraße 15", "Stuttgart", "70173"),
            DriversLicense.Of("B222111000DE", "Germany", new DateOnly(2005, 5, 12), new DateOnly(2026, 5, 12))
        ));

        // Customer 7: Felix Hoffmann - Mid-age from Düsseldorf
        customers.Add(CreateCustomer(
            CustomerName.Of("Felix", "Hoffmann"),
            Email.From("f.hoffmann@web.de"),
            PhoneNumber.From("+49 211 66677788"),
            BirthDate.Of(1990, 12, 5),
            Address.Of("Königsallee 60", "Düsseldorf", "40212"),
            DriversLicense.Of("B666777888DE", "Germany", new DateOnly(2012, 1, 15), new DateOnly(2027, 1, 15))
        ));

        // Customer 8: Mia Schäfer - Young professional from Leipzig
        customers.Add(CreateCustomer(
            CustomerName.Of("Mia", "Schäfer"),
            Email.From("mia.schaefer@email.de"),
            PhoneNumber.From("+49 341 44455566"),
            BirthDate.Of(1996, 6, 27),
            Address.Of("Petersstraße 30", "Leipzig", "04109"),
            DriversLicense.Of("B444555666DE", "Germany", new DateOnly(2016, 8, 22), new DateOnly(2031, 8, 22))
        ));

        // Customer 9: Leon Meyer - Senior citizen from Bremen
        customers.Add(CreateCustomer(
            CustomerName.Of("Leon", "Meyer"),
            Email.From("leon.meyer@posteo.de"),
            PhoneNumber.From("+49 421 11122233"),
            BirthDate.Of(1955, 2, 14),
            Address.Of("Böttcherstraße 8", "Bremen", "28195"),
            DriversLicense.Of("B111222333DE", "Germany", new DateOnly(1975, 6, 10), new DateOnly(2026, 6, 10))
        ));

        // Customer 10: Lena Koch - Mid-career from Hannover
        customers.Add(CreateCustomer(
            CustomerName.Of("Lena", "Koch"),
            Email.From("lena.koch@yahoo.de"),
            PhoneNumber.From("+49 511 88899900"),
            BirthDate.Of(1987, 10, 3),
            Address.Of("Kröpcke 10", "Hannover", "30159"),
            DriversLicense.Of("B888999000DE", "Germany", new DateOnly(2007, 11, 5), new DateOnly(2027, 11, 5))
        ));

        // Customer 11: Paul Richter - Young entrepreneur from Dresden
        customers.Add(CreateCustomer(
            CustomerName.Of("Paul", "Richter"),
            Email.From("paul.richter@gmail.com"),
            PhoneNumber.From("+49 351 55566677"),
            BirthDate.Of(1992, 8, 19),
            Address.Of("Prager Straße 20", "Dresden", "01069"),
            DriversLicense.Of("B555666777DE", "Germany", new DateOnly(2013, 4, 18), new DateOnly(2028, 4, 18))
        ));

        // Customer 12: Laura Zimmermann - Student from Freiburg
        customers.Add(CreateCustomer(
            CustomerName.Of("Laura", "Zimmermann"),
            Email.From("laura.zimmer@student.de"),
            PhoneNumber.From("+49 761 33322211"),
            BirthDate.Of(2002, 5, 25),
            Address.Of("Kaiser-Joseph-Straße 200", "Freiburg", "79098"),
            DriversLicense.Of("B333222111DE", "Germany", new DateOnly(2021, 7, 12), new DateOnly(2036, 7, 12))
        ));

        // Customer 13: David Klein - Suspended customer (late payment issue)
        var suspendedCustomer = CreateCustomer(
            CustomerName.Of("David", "Klein"),
            Email.From("david.klein@example.de"),
            PhoneNumber.From("+49 228 99988877"),
            BirthDate.Of(1983, 3, 8),
            Address.Of("Münsterplatz 12", "Bonn", "53111"),
            DriversLicense.Of("B999888777DE", "Germany", new DateOnly(2003, 12, 1), new DateOnly(2028, 12, 1))
        );
        suspendedCustomer.Suspend("Pending payment verification");
        customers.Add(suspendedCustomer);

        // Customer 14: Sarah Lang - Older driver from Nuremberg
        customers.Add(CreateCustomer(
            CustomerName.Of("Sarah", "Lang"),
            Email.From("sarah.lang@freenet.de"),
            PhoneNumber.From("+49 911 12312312"),
            BirthDate.Of(1970, 11, 20),
            Address.Of("Kaiserstraße 50", "Nürnberg", "90403"),
            DriversLicense.Of("B123123123DE", "Germany", new DateOnly(1990, 3, 15), new DateOnly(2027, 3, 15))
        ));

        // Customer 15: Tim Schwarz - Recent driver
        customers.Add(CreateCustomer(
            CustomerName.Of("Tim", "Schwarz"),
            Email.From("tim.schwarz@mailbox.org"),
            PhoneNumber.From("+49 201 45645645"),
            BirthDate.Of(1998, 7, 7),
            Address.Of("Limbecker Platz 1", "Essen", "45127"),
            DriversLicense.Of("B456456456DE", "Germany", new DateOnly(2018, 10, 20), new DateOnly(2033, 10, 20))
        ));

        return customers;
    }

    private static Customer CreateCustomer(
        CustomerName name,
        Email email,
        PhoneNumber phone,
        BirthDate birthDate,
        Address address,
        DriversLicense license)
    {
        var customer = new Customer();
        customer.Register(name, email, phone, birthDate, address, license);
        return customer;
    }
}
