using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class productChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                table: "ProductVariantOption",
                newName: "DisplayValue");

            migrationBuilder.RenameColumn(
                name: "OptionValue",
                table: "ProductVariantOption",
                newName: "ActualValue");

            migrationBuilder.AddColumn<string>(
                name: "PricingStrategy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesUnitMode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesUnitPackLabel",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesUnitPackWeight",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SalesUnitPricePerWeightUnit",
                table: "Products",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalesUnitWeightUnit",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricingStrategy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesUnitMode",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesUnitPackLabel",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesUnitPackWeight",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesUnitPricePerWeightUnit",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SalesUnitWeightUnit",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "DisplayValue",
                table: "ProductVariantOption",
                newName: "Value");

            migrationBuilder.RenameColumn(
                name: "ActualValue",
                table: "ProductVariantOption",
                newName: "OptionValue");
        }
    }
}
