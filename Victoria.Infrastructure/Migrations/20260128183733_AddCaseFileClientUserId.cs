using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseFileClientUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientUserId",
                table: "CaseFiles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_ClientUserId",
                table: "CaseFiles",
                column: "ClientUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CaseFiles_ClientUserId",
                table: "CaseFiles");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                table: "CaseFiles");
        }
    }
}
