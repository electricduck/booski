using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _001 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostLogs",
                columns: table => new
                {
                    RecordKey = table.Column<string>(type: "TEXT", nullable: false),
                    Telegram_Id = table.Column<string>(type: "TEXT", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Version = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostLogs", x => x.RecordKey);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostLogs");
        }
    }
}
