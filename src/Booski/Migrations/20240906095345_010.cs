using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _010 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "X_PostId",
                table: "PostLogs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "X_PostId",
                table: "PostLogs");
        }
    }
}
