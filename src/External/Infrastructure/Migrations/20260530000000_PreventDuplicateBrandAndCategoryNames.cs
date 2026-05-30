using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260530000000_PreventDuplicateBrandAndCategoryNames")]
    public partial class PreventDuplicateBrandAndCategoryNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [Brands] SET [Name] = LTRIM(RTRIM([Name])) WHERE [Name] <> LTRIM(RTRIM([Name]));");
            migrationBuilder.Sql("UPDATE [Categories] SET [Name] = LTRIM(RTRIM([Name])) WHERE [Name] <> LTRIM(RTRIM([Name]));");

            migrationBuilder.Sql(@"
WITH DuplicateBrands AS
(
    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY LOWER([Name]) ORDER BY [Id]) AS [RowNumber]
    FROM [Brands]
    WHERE [IsDeleted] = 0
)
UPDATE [Brands]
SET [IsDeleted] = 1,
    [ModifiedTime] = SYSUTCDATETIME()
WHERE [Id] IN (SELECT [Id] FROM DuplicateBrands WHERE [RowNumber] > 1);");

            migrationBuilder.Sql(@"
WITH DuplicateCategories AS
(
    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY LOWER([Name]) ORDER BY [Id]) AS [RowNumber]
    FROM [Categories]
    WHERE [IsDeleted] = 0
)
UPDATE [Categories]
SET [IsDeleted] = 1,
    [ModifiedTime] = SYSUTCDATETIME()
WHERE [Id] IN (SELECT [Id] FROM DuplicateCategories WHERE [RowNumber] > 1);");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_Name",
                table: "Brands",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Brands_Name",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");
        }
    }
}
