using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Result",
                table: "ExamResults");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "ExamResults",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notes",
                table: "ExamResults");

            migrationBuilder.AddColumn<string>(
                name: "Result",
                table: "ExamResults",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
