using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260607010000_AddPromotionCodeIndex")]
    public partial class AddPromotionCodeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE [dbo].[Promotions]
SET [PromotionCode] = UPPER(LTRIM(RTRIM([PromotionCode])))
WHERE [PromotionCode] <> UPPER(LTRIM(RTRIM([PromotionCode])));

IF NOT EXISTS (
    SELECT 1
    FROM [sys].[indexes]
    WHERE [name] = N'IX_Promotions_PromotionCode'
      AND [object_id] = OBJECT_ID(N'[dbo].[Promotions]')
)
BEGIN
    CREATE INDEX [IX_Promotions_PromotionCode]
    ON [dbo].[Promotions] ([PromotionCode]);
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1
    FROM [sys].[indexes]
    WHERE [name] = N'IX_Promotions_PromotionCode'
      AND [object_id] = OBJECT_ID(N'[dbo].[Promotions]')
)
BEGIN
    DROP INDEX [IX_Promotions_PromotionCode]
    ON [dbo].[Promotions];
END;");
        }
    }
}
