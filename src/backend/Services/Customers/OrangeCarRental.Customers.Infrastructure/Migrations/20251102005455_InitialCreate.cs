using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "customers");

            migrationBuilder.CreateTable(
                name: "Customers",
                schema: "customers",
                columns: table => new
                {
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RegisteredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address_PostalCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Address_Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DriversLicense_ExpiryDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DriversLicense_IssueCountry = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DriversLicense_IssueDate = table.Column<DateOnly>(type: "date", nullable: false),
                    DriversLicense_LicenseNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_DateOfBirth",
                schema: "customers",
                table: "Customers",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                schema: "customers",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                schema: "customers",
                table: "Customers",
                column: "PhoneNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_RegisteredAtUtc",
                schema: "customers",
                table: "Customers",
                column: "RegisteredAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Status",
                schema: "customers",
                table: "Customers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_City",
                schema: "customers",
                table: "Customers",
                column: "Address_City");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PostalCode",
                schema: "customers",
                table: "Customers",
                column: "Address_PostalCode");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_LicenseExpiryDate",
                schema: "customers",
                table: "Customers",
                column: "DriversLicense_ExpiryDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Customers",
                schema: "customers");
        }
    }
}
