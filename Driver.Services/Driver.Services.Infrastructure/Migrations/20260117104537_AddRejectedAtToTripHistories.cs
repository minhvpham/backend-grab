using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Driver.Services.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectedAtToTripHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RejectedAt",
                table: "TripHistories",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectedAt",
                table: "TripHistories");
        }
    }
}
