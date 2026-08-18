using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class basket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BasketId",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdminNotes",
                table: "Baskets",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConvertedOrderId",
                table: "Baskets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderAt",
                table: "Baskets",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Baskets",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BasketId",
                table: "Orders",
                column: "BasketId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_GuestId",
                table: "Baskets",
                column: "GuestId");

            migrationBuilder.CreateIndex(
                name: "IX_Baskets_Status",
                table: "Baskets",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_BasketId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_GuestId",
                table: "Baskets");

            migrationBuilder.DropIndex(
                name: "IX_Baskets_Status",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "BasketId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AdminNotes",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "ConvertedOrderId",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "LastReminderAt",
                table: "Baskets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Baskets");
        }
    }
}
