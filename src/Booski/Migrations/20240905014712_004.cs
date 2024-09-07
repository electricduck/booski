using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telegram_Id",
                table: "PostLogs");

            migrationBuilder.AddColumn<long>(
                name: "Telegram_ChatId",
                table: "PostLogs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Telegram_MessageId",
                table: "PostLogs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telegram_ChatId",
                table: "PostLogs");

            migrationBuilder.DropColumn(
                name: "Telegram_MessageId",
                table: "PostLogs");

            migrationBuilder.AddColumn<string>(
                name: "Telegram_Id",
                table: "PostLogs",
                type: "TEXT",
                nullable: true);
        }
    }
}
