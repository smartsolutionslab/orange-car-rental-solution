using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountHolderValueConverter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LineItems",
                schema: "payments",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "SepaMandates",
                schema: "payments",
                columns: table => new
                {
                    SepaMandateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MandateReference = table.Column<string>(type: "nvarchar(35)", maxLength: 35, nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    BIC = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    AccountHolder = table.Column<string>(type: "nvarchar(140)", maxLength: 140, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SepaMandates", x => x.SepaMandateId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SepaMandates_CustomerId",
                schema: "payments",
                table: "SepaMandates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_SepaMandates_CustomerId_Status",
                schema: "payments",
                table: "SepaMandates",
                columns: new[] { "CustomerId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_SepaMandates_MandateReference",
                schema: "payments",
                table: "SepaMandates",
                column: "MandateReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SepaMandates_Status",
                schema: "payments",
                table: "SepaMandates",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SepaMandates",
                schema: "payments");

            migrationBuilder.AlterColumn<string>(
                name: "LineItems",
                schema: "payments",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
