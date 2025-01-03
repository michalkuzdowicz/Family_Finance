using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class FinancialTarget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Transactions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FinancialTarget",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TargetAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FamilyGroupID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialTarget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialTarget_FamilyGroups_FamilyGroupID",
                        column: x => x.FamilyGroupID,
                        principalTable: "FamilyGroups",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "TargetTransaction",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FinancialTargetId = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetTransaction", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetTransaction_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TargetTransaction_FinancialTarget_FinancialTargetId",
                        column: x => x.FinancialTargetId,
                        principalTable: "FinancialTarget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FinancialTarget_FamilyGroupID",
                table: "FinancialTarget",
                column: "FamilyGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTransaction_FinancialTargetId",
                table: "TargetTransaction",
                column: "FinancialTargetId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetTransaction_UserId",
                table: "TargetTransaction",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TargetTransaction");

            migrationBuilder.DropTable(
                name: "FinancialTarget");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Transactions");
        }
    }
}
