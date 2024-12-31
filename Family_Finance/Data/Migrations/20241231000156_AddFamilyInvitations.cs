using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Family_Finance.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFamilyInvitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FamilyInvitations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InviterId = table.Column<string>(nullable: false),
                    InviteeEmail = table.Column<string>(nullable: false),
                    IsAccepted = table.Column<bool>(nullable: false),
                    InvitationDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyInvitations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FamilyInvitations_AspNetUsers_InviterId",
                        column: x => x.InviterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FamilyInvitations_InviterId",
                table: "FamilyInvitations",
                column: "InviterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FamilyInvitations");
        }

    }
}
