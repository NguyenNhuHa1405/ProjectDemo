using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AspMvc.Migrations
{
    /// <inheritdoc />
    public partial class Commnet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoomComment",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomComment", x => x.RoomId);
                    table.ForeignKey(
                        name: "FK_RoomComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MessageComment",
                columns: table => new
                {
                    MessageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RoomId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageComment", x => x.MessageId);
                    table.ForeignKey(
                        name: "FK_MessageComment_RoomComment_RoomId",
                        column: x => x.RoomId,
                        principalTable: "RoomComment",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MessageComment_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageComment_RoomId",
                table: "MessageComment",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageComment_UserId",
                table: "MessageComment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomComment_UserId",
                table: "RoomComment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageComment");

            migrationBuilder.DropTable(
                name: "RoomComment");
        }
    }
}
