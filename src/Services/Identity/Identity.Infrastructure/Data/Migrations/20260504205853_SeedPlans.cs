using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Identity.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Plans",
                columns: new[] { "PlanId", "PlanName" },
                values: new object[,]
                {
                    { 1, "Basic" },
                    { 2, "Pro" },
                    { 3, "Enterprise" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "PlanId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "PlanId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Plans",
                keyColumn: "PlanId",
                keyValue: 3);
        }
    }
}
