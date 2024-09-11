using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booski.Migrations
{
    /// <inheritdoc />
    public partial class _015 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileCaches",
                table: "FileCaches");

            migrationBuilder.DropColumn(
                name: "Ref",
                table: "FileCaches");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileCaches",
                table: "FileCaches",
                column: "Uri");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_FileCaches",
                table: "FileCaches");

            migrationBuilder.AddColumn<string>(
                name: "Ref",
                table: "FileCaches",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileCaches",
                table: "FileCaches",
                column: "Ref");
        }
    }
}
