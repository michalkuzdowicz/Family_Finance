using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixFinanceTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CurrentAmount",
                table: "FinancialTarget",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentAmount",
                table: "FinancialTarget");
        }
    }
}
