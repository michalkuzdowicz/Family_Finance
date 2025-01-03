using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTargetTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FinancialTargetID",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FinacialTargetId",
                table: "TargetTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "TargetTransaction",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FinancialTargetID",
                table: "Transactions",
                column: "FinancialTargetID");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTransaction_TransactionId",
                table: "TargetTransaction",
                column: "TransactionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransaction_Transactions_TransactionId",
                table: "TargetTransaction",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_FinancialTarget_FinancialTargetID",
                table: "Transactions",
                column: "FinancialTargetID",
                principalTable: "FinancialTarget",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransaction_Transactions_TransactionId",
                table: "TargetTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_FinancialTarget_FinancialTargetID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_FinancialTargetID",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TargetTransaction_TransactionId",
                table: "TargetTransaction");

            migrationBuilder.DropColumn(
                name: "FinancialTargetID",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FinacialTargetId",
                table: "TargetTransaction");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "TargetTransaction");
        }
    }
}
