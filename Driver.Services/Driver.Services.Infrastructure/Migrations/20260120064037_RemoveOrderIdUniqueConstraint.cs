using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driver.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOrderIdUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TripHistories_OrderId",
                table: "TripHistories");

            migrationBuilder.CreateIndex(
                name: "IX_TripHistories_OrderId",
                table: "TripHistories",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TripHistories_OrderId",
                table: "TripHistories");

            migrationBuilder.CreateIndex(
                name: "IX_TripHistories_OrderId",
                table: "TripHistories",
                column: "OrderId",
                unique: true);
        }
    }
}
