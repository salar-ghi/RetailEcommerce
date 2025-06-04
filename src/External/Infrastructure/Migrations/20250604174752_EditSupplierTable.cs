using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditSupplierTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "ContactInfo",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "ContactPhone",
                table: "Suppliers",
                newName: "Info");

            migrationBuilder.RenameColumn(
                name: "ContactName",
                table: "Suppliers",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Suppliers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "Info",
                table: "Suppliers",
                newName: "ContactPhone");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Suppliers",
                newName: "ContactName");

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContactInfo",
                table: "Suppliers",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
