using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyGroupTransactions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(21)",
                maxLength: 21,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "FamilyGroupID",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FamilyGroups",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HeadOfFamilyID = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyGroups", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FamilyGroupID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Transactions_FamilyGroups_FamilyGroupID",
                        column: x => x.FamilyGroupID,
                        principalTable: "FamilyGroups",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_FamilyGroupID",
                table: "AspNetUsers",
                column: "FamilyGroupID");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_FamilyGroupID",
                table: "Transactions",
                column: "FamilyGroupID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_FamilyGroups_FamilyGroupID",
                table: "AspNetUsers",
                column: "FamilyGroupID",
                principalTable: "FamilyGroups",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_FamilyGroups_FamilyGroupID",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "FamilyGroups");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_FamilyGroupID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "FamilyGroupID",
                table: "AspNetUsers");
        }
    }
}
