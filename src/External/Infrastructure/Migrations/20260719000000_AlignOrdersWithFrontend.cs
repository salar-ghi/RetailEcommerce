using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class AlignOrdersWithFrontend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "Notes", table: "Orders", type: "nvarchar(1000)", maxLength: 1000, nullable: true);
            migrationBuilder.AddColumn<int>(name: "Source", table: "Orders", type: "int", nullable: false, defaultValue: 0);

            migrationBuilder.AddColumn<string>(name: "SaleUnit", table: "OrderItems", type: "nvarchar(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<string>(name: "WeightUnit", table: "OrderItems", type: "nvarchar(20)", maxLength: 20, nullable: true);
            migrationBuilder.AddColumn<int>(name: "SpaceId", table: "OrderItems", type: "int", nullable: true);
            migrationBuilder.AddColumn<string>(name: "SpaceName", table: "OrderItems", type: "nvarchar(150)", maxLength: 150, nullable: true);
            migrationBuilder.AddColumn<int>(name: "ZoneId", table: "OrderItems", type: "int", nullable: true);
            migrationBuilder.AddColumn<string>(name: "ZoneName", table: "OrderItems", type: "nvarchar(150)", maxLength: 150, nullable: true);
            migrationBuilder.AddColumn<int>(name: "ShelfId", table: "OrderItems", type: "int", nullable: true);
            migrationBuilder.AddColumn<string>(name: "ShelfCode", table: "OrderItems", type: "nvarchar(80)", maxLength: 80, nullable: true);

            migrationBuilder.AddColumn<DateTime>(name: "DueDate", table: "Payments", type: "datetime2", nullable: true);
            migrationBuilder.AddColumn<string>(name: "FinanceAccountId", table: "Payments", type: "nvarchar(100)", maxLength: 100, nullable: true);
            migrationBuilder.AddColumn<string>(name: "BranchId", table: "Payments", type: "nvarchar(100)", maxLength: 100, nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "Notes", table: "Orders");
            migrationBuilder.DropColumn(name: "Source", table: "Orders");
            migrationBuilder.DropColumn(name: "SaleUnit", table: "OrderItems");
            migrationBuilder.DropColumn(name: "WeightUnit", table: "OrderItems");
            migrationBuilder.DropColumn(name: "SpaceId", table: "OrderItems");
            migrationBuilder.DropColumn(name: "SpaceName", table: "OrderItems");
            migrationBuilder.DropColumn(name: "ZoneId", table: "OrderItems");
            migrationBuilder.DropColumn(name: "ZoneName", table: "OrderItems");
            migrationBuilder.DropColumn(name: "ShelfId", table: "OrderItems");
            migrationBuilder.DropColumn(name: "ShelfCode", table: "OrderItems");
            migrationBuilder.DropColumn(name: "DueDate", table: "Payments");
            migrationBuilder.DropColumn(name: "FinanceAccountId", table: "Payments");
            migrationBuilder.DropColumn(name: "BranchId", table: "Payments");
        }
    }
}
