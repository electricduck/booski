﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _005 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Telegram_MessageCount",
                table: "PostLogs",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telegram_MessageCount",
                table: "PostLogs");
        }
    }
}
