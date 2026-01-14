using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageCourses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationWeeks",
                table: "LanguageCourses");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "LanguageCourses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "LanguageCourses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "LanguageCourses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "LanguageCourses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LanguageCourses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "LanguageCourses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "LanguageCourses");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "LanguageCourses");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "LanguageCourses");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LanguageCourses");

            migrationBuilder.AddColumn<int>(
                name: "DurationWeeks",
                table: "LanguageCourses",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
