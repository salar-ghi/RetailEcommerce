using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class banner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "BannerPlacementMap",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "BannerPlacementMap",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "BannerPlacementMap",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "BannerPlacementMap",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "BannerPlacementMap",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedTime",
                table: "BannerPlacementMap",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "BannerPlacementMap");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "BannerPlacementMap");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "BannerPlacementMap");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "BannerPlacementMap");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "BannerPlacementMap");

            migrationBuilder.DropColumn(
                name: "ModifiedTime",
                table: "BannerPlacementMap");
        }
    }
}
