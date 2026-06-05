using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260605000000_FixTagCrudContract")]
    public partial class FixTagCrudContract : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tags",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.Sql("UPDATE [Tags] SET [Name] = LTRIM(RTRIM([Name])), [Color] = LOWER(LTRIM(RTRIM([Color]))) WHERE [IsDeleted] = 0;");

            migrationBuilder.Sql(@"
WITH DuplicateTags AS
(
    SELECT [Id], ROW_NUMBER() OVER (PARTITION BY LOWER([Name]) ORDER BY [Id]) AS [RowNumber]
    FROM [Tags]
    WHERE [IsDeleted] = 0
)
UPDATE [Tags]
SET [IsDeleted] = 1,
    [ModifiedTime] = SYSUTCDATETIME()
WHERE [Id] IN (SELECT [Id] FROM DuplicateTags WHERE [RowNumber] > 1);");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tags");
        }
    }
}
