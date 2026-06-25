using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "InventoryBatch",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InventoryBatch_SupplierId",
                table: "InventoryBatch",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryBatch_Suppliers_SupplierId",
                table: "InventoryBatch",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InventoryBatch_Suppliers_SupplierId",
                table: "InventoryBatch");

            migrationBuilder.DropIndex(
                name: "IX_InventoryBatch_SupplierId",
                table: "InventoryBatch");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "InventoryBatch");
        }
    }
}
