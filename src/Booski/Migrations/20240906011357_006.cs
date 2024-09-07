using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _006 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsernameMaps",
                columns: table => new
                {
                    Bluesky_Did = table.Column<string>(type: "TEXT", nullable: false),
                    Mastodon_Handle = table.Column<string>(type: "TEXT", nullable: false),
                    Telegram_Handle = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsernameMaps", x => x.Bluesky_Did);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsernameMaps");
        }
    }
}
