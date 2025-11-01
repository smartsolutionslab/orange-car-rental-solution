using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationCodesToReservation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DropoffLocationCode",
                schema: "reservations",
                table: "Reservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickupLocationCode",
                schema: "reservations",
                table: "Reservations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_DropoffLocationCode",
                schema: "reservations",
                table: "Reservations",
                column: "DropoffLocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_PickupLocationCode",
                schema: "reservations",
                table: "Reservations",
                column: "PickupLocationCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Reservations_DropoffLocationCode",
                schema: "reservations",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_PickupLocationCode",
                schema: "reservations",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "DropoffLocationCode",
                schema: "reservations",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "PickupLocationCode",
                schema: "reservations",
                table: "Reservations");
        }
    }
}
