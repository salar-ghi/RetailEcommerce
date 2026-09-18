using Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260918000000_AddProductContentBlocks")]
public partial class AddProductContentBlocks : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "ProductContentBlocks",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                ProductId = table.Column<long>(type: "bigint", nullable: false),
                ClientId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Text = table.Column<string>(type: "nvarchar(max)", maxLength: 10000, nullable: true),
                ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                Caption = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                SortOrder = table.Column<int>(type: "int", nullable: false),
                CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ModifiedTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                IsDeleted = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductContentBlocks", x => x.Id);
                table.ForeignKey(
                    name: "FK_ProductContentBlocks_Products_ProductId",
                    column: x => x.ProductId,
                    principalTable: "Products",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ProductContentBlocks_ProductId_SortOrder",
            table: "ProductContentBlocks",
            columns: new[] { "ProductId", "SortOrder" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "ProductContentBlocks");
    }
}
