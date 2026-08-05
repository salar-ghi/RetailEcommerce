using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixProductAttributeValueRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId1",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_ProductId1",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "ProductId1",
                table: "ProductAttributeValues");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ProductId1",
                table: "ProductAttributeValues",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_ProductId1",
                table: "ProductAttributeValues",
                column: "ProductId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_Products_ProductId1",
                table: "ProductAttributeValues",
                column: "ProductId1",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
