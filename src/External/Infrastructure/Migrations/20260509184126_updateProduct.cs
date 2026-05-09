using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ProductVariantOption",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OptionValue",
                table: "ProductVariantOption",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAdjustment",
                table: "ProductVariantOption",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sku",
                table: "ProductVariantOption",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "ProductVariantOption",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SpaceId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StorageLocationNote",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ZoneId",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "ProductVariantOption");

            migrationBuilder.DropColumn(
                name: "OptionValue",
                table: "ProductVariantOption");

            migrationBuilder.DropColumn(
                name: "PriceAdjustment",
                table: "ProductVariantOption");

            migrationBuilder.DropColumn(
                name: "Sku",
                table: "ProductVariantOption");

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "ProductVariantOption");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SpaceId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StorageLocationNote",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ZoneId",
                table: "Products");
        }
    }
}
