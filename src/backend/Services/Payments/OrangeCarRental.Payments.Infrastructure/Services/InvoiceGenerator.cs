using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Services;

/// <summary>
///     PDF invoice generator using QuestPDF.
///     Generates German-compliant invoices (§14 UStG).
/// </summary>
public sealed class InvoiceGenerator : IInvoiceGenerator
{
    private static readonly CultureInfo GermanCulture = new("de-DE");

    static InvoiceGenerator()
    {
        // QuestPDF Community License
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GeneratePdf(Invoice invoice)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginTop(2, Unit.Centimetre);
                page.MarginBottom(2, Unit.Centimetre);
                page.MarginHorizontal(2.5f, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ComposeHeader(c, invoice));
                page.Content().Element(c => ComposeContent(c, invoice));
                page.Footer().Element(c => ComposeFooter(c, invoice));
            });
        });

        return document.GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, Invoice invoice)
    {
        container.Column(column =>
        {
            // Company logo and name
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(invoice.SellerName)
                        .Bold().FontSize(18).FontColor(Colors.Orange.Medium);
                    c.Item().Text("Autovermietung")
                        .FontSize(12).FontColor(Colors.Grey.Darken2);
                });

                row.ConstantItem(150).AlignRight().Column(c =>
                {
                    c.Item().Text("RECHNUNG")
                        .Bold().FontSize(20).FontColor(Colors.Grey.Darken3);
                    c.Item().Text($"Nr. {invoice.InvoiceNumber}")
                        .FontSize(12);
                });
            });

            column.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Orange.Medium);

            // Address block
            column.Item().PaddingTop(15).Row(row =>
            {
                // Sender address (small line)
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text($"{invoice.SellerName} · {invoice.SellerStreet} · {invoice.SellerPostalCode} {invoice.SellerCity}")
                        .FontSize(7).FontColor(Colors.Grey.Darken1);

                    c.Item().PaddingTop(5);

                    // Customer address
                    c.Item().Text(invoice.CustomerName).FontSize(11);
                    c.Item().Text(invoice.CustomerStreet);
                    c.Item().Text($"{invoice.CustomerPostalCode} {invoice.CustomerCity}");
                    c.Item().Text(invoice.CustomerCountry);

                    if (!string.IsNullOrEmpty(invoice.CustomerVatId))
                    {
                        c.Item().PaddingTop(3).Text($"USt-IdNr.: {invoice.CustomerVatId}")
                            .FontSize(9).FontColor(Colors.Grey.Darken2);
                    }
                });

                // Invoice details box
                row.ConstantItem(180).Border(1).BorderColor(Colors.Grey.Lighten1)
                    .Padding(10).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Rechnungsdatum:").Bold();
                            r.ConstantItem(80).AlignRight()
                                .Text(invoice.InvoiceDate.ToString("dd.MM.yyyy", GermanCulture));
                        });

                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Leistungsdatum:").Bold();
                            r.ConstantItem(80).AlignRight()
                                .Text(invoice.ServiceDate.ToString("dd.MM.yyyy", GermanCulture));
                        });

                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Fällig am:").Bold();
                            r.ConstantItem(80).AlignRight()
                                .Text(invoice.DueDate.ToString("dd.MM.yyyy", GermanCulture));
                        });

                        c.Item().PaddingTop(5).Row(r =>
                        {
                            r.RelativeItem().Text("Kunden-Nr.:").Bold();
                            r.ConstantItem(80).AlignRight()
                                .Text(invoice.CustomerId.ToString()[..8].ToUpperInvariant());
                        });

                        c.Item().Row(r =>
                        {
                            r.RelativeItem().Text("Reservierung:").Bold();
                            r.ConstantItem(80).AlignRight()
                                .Text(invoice.ReservationId.ToString()[..8].ToUpperInvariant());
                        });
                    });
            });
        });
    }

    private static void ComposeContent(IContainer container, Invoice invoice)
    {
        container.PaddingVertical(20).Column(column =>
        {
            // Greeting
            column.Item().Text($"Sehr geehrte(r) {invoice.CustomerName},")
                .FontSize(10);

            column.Item().PaddingTop(5).Text("vielen Dank für Ihre Buchung. Nachfolgend finden Sie die Rechnung für Ihre Fahrzeugmiete:")
                .FontSize(10);

            // Line items table
            column.Item().PaddingTop(15).Element(c => ComposeTable(c, invoice));

            // Totals
            column.Item().PaddingTop(10).AlignRight().Width(250).Column(totalsColumn =>
            {
                totalsColumn.Item().Row(row =>
                {
                    row.RelativeItem().Text("Nettobetrag:");
                    row.ConstantItem(100).AlignRight()
                        .Text(FormatCurrency(invoice.TotalNet, invoice.CurrencyCode));
                });

                totalsColumn.Item().Row(row =>
                {
                    row.RelativeItem().Text("MwSt. (19%):");
                    row.ConstantItem(100).AlignRight()
                        .Text(FormatCurrency(invoice.TotalVat, invoice.CurrencyCode));
                });

                totalsColumn.Item().PaddingTop(5).LineHorizontal(1);

                totalsColumn.Item().PaddingTop(5).Row(row =>
                {
                    row.RelativeItem().Text("Gesamtbetrag:").Bold().FontSize(12);
                    row.ConstantItem(100).AlignRight()
                        .Text(FormatCurrency(invoice.TotalGross, invoice.CurrencyCode))
                        .Bold().FontSize(12);
                });
            });

            // Payment information
            column.Item().PaddingTop(25).Column(paymentCol =>
            {
                paymentCol.Item().Text("Zahlungsinformationen").Bold().FontSize(11);
                paymentCol.Item().PaddingTop(5).Text(
                    $"Bitte überweisen Sie den Betrag von {FormatCurrency(invoice.TotalGross, invoice.CurrencyCode)} " +
                    $"bis zum {invoice.DueDate.ToString("dd.MM.yyyy", GermanCulture)} auf folgendes Konto:");

                paymentCol.Item().PaddingTop(10).Border(1).BorderColor(Colors.Grey.Lighten2)
                    .Background(Colors.Grey.Lighten4).Padding(10).Column(bankCol =>
                    {
                        bankCol.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text("Kontoinhaber:").Bold();
                            r.RelativeItem().Text(invoice.SellerName);
                        });
                        bankCol.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text("IBAN:").Bold();
                            r.RelativeItem().Text("DE89 3704 0044 0532 0130 00");
                        });
                        bankCol.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text("BIC:").Bold();
                            r.RelativeItem().Text("COBADEFFXXX");
                        });
                        bankCol.Item().Row(r =>
                        {
                            r.ConstantItem(100).Text("Verwendungszweck:").Bold();
                            r.RelativeItem().Text(invoice.InvoiceNumber.Value);
                        });
                    });
            });

            // Legal note
            column.Item().PaddingTop(20).Text(
                "Hinweis: Diese Rechnung wurde maschinell erstellt und ist ohne Unterschrift gültig.")
                .FontSize(8).FontColor(Colors.Grey.Darken2);
        });
    }

    private static void ComposeTable(IContainer container, Invoice invoice)
    {
        container.Table(table =>
        {
            // Column definitions
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(30);  // Position
                columns.RelativeColumn(4);   // Description
                columns.ConstantColumn(50);  // Quantity
                columns.ConstantColumn(80);  // Unit price
                columns.ConstantColumn(60);  // VAT %
                columns.ConstantColumn(80);  // Total
            });

            // Header
            table.Header(header =>
            {
                header.Cell().Background(Colors.Orange.Medium).Padding(5)
                    .Text("Pos.").Bold().FontColor(Colors.White);
                header.Cell().Background(Colors.Orange.Medium).Padding(5)
                    .Text("Beschreibung").Bold().FontColor(Colors.White);
                header.Cell().Background(Colors.Orange.Medium).Padding(5).AlignCenter()
                    .Text("Menge").Bold().FontColor(Colors.White);
                header.Cell().Background(Colors.Orange.Medium).Padding(5).AlignRight()
                    .Text("Einzelpreis").Bold().FontColor(Colors.White);
                header.Cell().Background(Colors.Orange.Medium).Padding(5).AlignCenter()
                    .Text("MwSt.").Bold().FontColor(Colors.White);
                header.Cell().Background(Colors.Orange.Medium).Padding(5).AlignRight()
                    .Text("Gesamt").Bold().FontColor(Colors.White);
            });

            // Rows
            foreach (var item in invoice.LineItems)
            {
                var backgroundColor = item.Position % 2 == 0
                    ? Colors.Grey.Lighten4
                    : Colors.White;

                table.Cell().Background(backgroundColor).Padding(5)
                    .Text(item.Position.ToString());

                table.Cell().Background(backgroundColor).Padding(5).Column(c =>
                {
                    c.Item().Text(item.Description);
                    if (item.ServicePeriodStart.HasValue && item.ServicePeriodEnd.HasValue)
                    {
                        c.Item().Text($"({item.ServicePeriodStart.Value.ToString("dd.MM.yyyy", GermanCulture)} - " +
                                     $"{item.ServicePeriodEnd.Value.ToString("dd.MM.yyyy", GermanCulture)})")
                            .FontSize(8).FontColor(Colors.Grey.Darken2);
                    }
                });

                table.Cell().Background(backgroundColor).Padding(5).AlignCenter()
                    .Text($"{item.Quantity} {item.Unit}");

                table.Cell().Background(backgroundColor).Padding(5).AlignRight()
                    .Text(FormatCurrency(item.UnitPriceNet, invoice.CurrencyCode));

                table.Cell().Background(backgroundColor).Padding(5).AlignCenter()
                    .Text($"{item.VatRate * 100:0}%");

                table.Cell().Background(backgroundColor).Padding(5).AlignRight()
                    .Text(FormatCurrency(item.TotalNet, invoice.CurrencyCode));
            }
        });
    }

    private static void ComposeFooter(IContainer container, Invoice invoice)
    {
        container.Column(column =>
        {
            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

            column.Item().PaddingTop(10).Row(row =>
            {
                // Company info
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text(invoice.SellerName).Bold().FontSize(8);
                    c.Item().Text($"{invoice.SellerStreet}").FontSize(8);
                    c.Item().Text($"{invoice.SellerPostalCode} {invoice.SellerCity}").FontSize(8);
                });

                // Contact
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Kontakt").Bold().FontSize(8);
                    c.Item().Text($"Tel: {invoice.SellerPhone}").FontSize(8);
                    c.Item().Text($"E-Mail: {invoice.SellerEmail}").FontSize(8);
                });

                // Legal info
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Handelsregister").Bold().FontSize(8);
                    c.Item().Text($"AG Berlin {invoice.TradeRegisterNumber}").FontSize(8);
                    c.Item().Text($"Geschäftsführer: {invoice.ManagingDirector}").FontSize(8);
                });

                // Tax info
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("Steuernummern").Bold().FontSize(8);
                    c.Item().Text($"USt-IdNr.: {invoice.VatId}").FontSize(8);
                    c.Item().Text($"Steuernr.: {invoice.TaxNumber}").FontSize(8);
                });
            });

            column.Item().PaddingTop(10).AlignCenter()
                .Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Darken2));
                    text.Span("Seite ");
                    text.CurrentPageNumber();
                    text.Span(" von ");
                    text.TotalPages();
                });
        });
    }

    private static string FormatCurrency(decimal amount, string currencyCode)
    {
        return currencyCode.ToUpperInvariant() switch
        {
            "EUR" => $"{amount.ToString("N2", GermanCulture)} €",
            "USD" => $"${amount.ToString("N2", GermanCulture)}",
            _ => $"{amount.ToString("N2", GermanCulture)} {currencyCode}"
        };
    }
}
