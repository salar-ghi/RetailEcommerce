using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260607000000_AddPromotionDiscountFields")]
    public partial class AddPromotionDiscountFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[Promotions]', N'IsEnabled') IS NULL
BEGIN
    ALTER TABLE [dbo].[Promotions]
    ADD [IsEnabled] bit NOT NULL CONSTRAINT [DF_Promotions_IsEnabled] DEFAULT CAST(1 AS bit);
END;

IF COL_LENGTH(N'[dbo].[Discounts]', N'MaxUsage') IS NULL
BEGIN
    ALTER TABLE [dbo].[Discounts]
    ADD [MaxUsage] int NULL;
END;

IF COL_LENGTH(N'[dbo].[Discounts]', N'UsedCount') IS NULL
BEGIN
    ALTER TABLE [dbo].[Discounts]
    ADD [UsedCount] int NOT NULL CONSTRAINT [DF_Discounts_UsedCount] DEFAULT 0;
END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH(N'[dbo].[Promotions]', N'IsEnabled') IS NOT NULL
BEGIN
    DECLARE @promotionDefaultConstraintName sysname;
    SELECT @promotionDefaultConstraintName = [dc].[name]
    FROM [sys].[default_constraints] [dc]
    INNER JOIN [sys].[columns] [c] ON [c].[default_object_id] = [dc].[object_id]
    WHERE [dc].[parent_object_id] = OBJECT_ID(N'[dbo].[Promotions]')
      AND [c].[name] = N'IsEnabled';

    IF @promotionDefaultConstraintName IS NOT NULL
        EXEC(N'ALTER TABLE [dbo].[Promotions] DROP CONSTRAINT [' + @promotionDefaultConstraintName + N']');

    ALTER TABLE [dbo].[Promotions] DROP COLUMN [IsEnabled];
END;

IF COL_LENGTH(N'[dbo].[Discounts]', N'UsedCount') IS NOT NULL
BEGIN
    DECLARE @discountDefaultConstraintName sysname;
    SELECT @discountDefaultConstraintName = [dc].[name]
    FROM [sys].[default_constraints] [dc]
    INNER JOIN [sys].[columns] [c] ON [c].[default_object_id] = [dc].[object_id]
    WHERE [dc].[parent_object_id] = OBJECT_ID(N'[dbo].[Discounts]')
      AND [c].[name] = N'UsedCount';

    IF @discountDefaultConstraintName IS NOT NULL
        EXEC(N'ALTER TABLE [dbo].[Discounts] DROP CONSTRAINT [' + @discountDefaultConstraintName + N']');

    ALTER TABLE [dbo].[Discounts] DROP COLUMN [UsedCount];
END;

IF COL_LENGTH(N'[dbo].[Discounts]', N'MaxUsage') IS NOT NULL
BEGIN
    ALTER TABLE [dbo].[Discounts] DROP COLUMN [MaxUsage];
END;");
        }
    }
}
