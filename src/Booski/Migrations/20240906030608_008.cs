using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mastodon_InstanceDomain",
                table: "PostLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Mastodon_StatusId",
                table: "PostLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Repository",
                table: "PostLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mastodon_InstanceDomain",
                table: "PostLogs");

            migrationBuilder.DropColumn(
                name: "Mastodon_StatusId",
                table: "PostLogs");

            migrationBuilder.DropColumn(
                name: "Repository",
                table: "PostLogs");
        }
    }
}
