using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class financial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The finance tables are created by the earlier 20260606000000_AddFinanceModule
            // migration. This migration only preserves the generated model snapshot/history so
            // Update-Database can advance without attempting to create the same tables again.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No schema changes were applied in Up. The finance tables are removed by
            // 20260606000000_AddFinanceModule when rolling back past that migration.
        }
    }
}
