using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTransactionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransaction_AspNetUsers_UserId",
                table: "TargetTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransaction_FinancialTarget_FinancialTargetId",
                table: "TargetTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransaction_Transactions_TransactionId",
                table: "TargetTransaction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetTransaction",
                table: "TargetTransaction");

            migrationBuilder.RenameTable(
                name: "TargetTransaction",
                newName: "TargetTransactions");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransaction_UserId",
                table: "TargetTransactions",
                newName: "IX_TargetTransactions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransaction_TransactionId",
                table: "TargetTransactions",
                newName: "IX_TargetTransactions_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransaction_FinancialTargetId",
                table: "TargetTransactions",
                newName: "IX_TargetTransactions_FinancialTargetId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "TargetTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetTransactions",
                table: "TargetTransactions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransactions_AspNetUsers_UserId",
                table: "TargetTransactions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransactions_FinancialTarget_FinancialTargetId",
                table: "TargetTransactions",
                column: "FinancialTargetId",
                principalTable: "FinancialTarget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransactions_Transactions_TransactionId",
                table: "TargetTransactions",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransactions_AspNetUsers_UserId",
                table: "TargetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransactions_FinancialTarget_FinancialTargetId",
                table: "TargetTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_TargetTransactions_Transactions_TransactionId",
                table: "TargetTransactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TargetTransactions",
                table: "TargetTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "TargetTransactions");

            migrationBuilder.RenameTable(
                name: "TargetTransactions",
                newName: "TargetTransaction");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransactions_UserId",
                table: "TargetTransaction",
                newName: "IX_TargetTransaction_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransactions_TransactionId",
                table: "TargetTransaction",
                newName: "IX_TargetTransaction_TransactionId");

            migrationBuilder.RenameIndex(
                name: "IX_TargetTransactions_FinancialTargetId",
                table: "TargetTransaction",
                newName: "IX_TargetTransaction_FinancialTargetId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TargetTransaction",
                table: "TargetTransaction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransaction_AspNetUsers_UserId",
                table: "TargetTransaction",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransaction_FinancialTarget_FinancialTargetId",
                table: "TargetTransaction",
                column: "FinancialTargetId",
                principalTable: "FinancialTarget",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TargetTransaction_Transactions_TransactionId",
                table: "TargetTransaction",
                column: "TransactionId",
                principalTable: "Transactions",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
