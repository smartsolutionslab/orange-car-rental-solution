using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBusinessCustomerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                schema: "customers",
                table: "Customers",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerType",
                schema: "customers",
                table: "Customers",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "PaymentTermsDays",
                schema: "customers",
                table: "Customers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VATId",
                schema: "customers",
                table: "Customers",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "CustomerType",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "PaymentTermsDays",
                schema: "customers",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "VATId",
                schema: "customers",
                table: "Customers");
        }
    }
}
