using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoices",
                schema: "payments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SellerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SellerStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SellerPostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    SellerCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SellerCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SellerEmail = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SellerPhone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TradeRegisterNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VatId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TaxNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ManagingDirector = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InvoiceNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    InvoiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerStreet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CustomerPostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CustomerCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CustomerVatId = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ReservationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CurrencyCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VoidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PdfDocument = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    LineItems = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                schema: "payments",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceDate",
                schema: "payments",
                table: "Invoices",
                column: "InvoiceDate");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_InvoiceNumber",
                schema: "payments",
                table: "Invoices",
                column: "InvoiceNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ReservationId",
                schema: "payments",
                table: "Invoices",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_Status",
                schema: "payments",
                table: "Invoices",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoices",
                schema: "payments");
        }
    }
}
