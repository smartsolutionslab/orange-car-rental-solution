using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "pricing");

            migrationBuilder.CreateTable(
                name: "PricingPolicies",
                schema: "pricing",
                columns: table => new
                {
                    PricingPolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveUntil = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    DailyRateNet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyRateVat = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PricingPolicies", x => x.PricingPolicyId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PricingPolicies_CategoryCode",
                schema: "pricing",
                table: "PricingPolicies",
                column: "CategoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_PricingPolicies_CategoryCode_IsActive",
                schema: "pricing",
                table: "PricingPolicies",
                columns: new[] { "CategoryCode", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_PricingPolicies_CategoryCode_LocationCode_IsActive",
                schema: "pricing",
                table: "PricingPolicies",
                columns: new[] { "CategoryCode", "LocationCode", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PricingPolicies",
                schema: "pricing");
        }
    }
}
