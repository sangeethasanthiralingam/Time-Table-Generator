using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Time_Table_Generator.Migrations
{
    /// <inheritdoc />
    public partial class timetableupdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "TimeTables",
                newName: "CreatedAt");

            migrationBuilder.AddColumn<string>(
                name: "Day",
                table: "TimeTables",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TimeTables",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "TimeTables",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "TimeTables",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "TimeTables");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "TimeTables");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TimeTables",
                newName: "Date");
        }
    }
}
