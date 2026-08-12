using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class attribute_code_datatype_unique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_Code",
                table: "AttributeDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_Code_DataType",
                table: "AttributeDefinitions",
                columns: new[] { "Code", "DataType" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttributeDefinitions_Code_DataType",
                table: "AttributeDefinitions");

            migrationBuilder.CreateIndex(
                name: "IX_AttributeDefinitions_Code",
                table: "AttributeDefinitions",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");
        }
    }
}
