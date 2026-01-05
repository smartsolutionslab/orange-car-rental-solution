using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Payments.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignConstructorParameterNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationId",
                schema: "payments",
                table: "Invoices",
                newName: "ReservationIdentifier");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReservationIdentifier",
                schema: "payments",
                table: "Invoices",
                newName: "ReservationId");
        }
    }
}
