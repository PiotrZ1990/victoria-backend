using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedChecklistTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ApplicationDocumentChecklists",
                columns: new[] { "Id", "DocumentType", "IsRequired" },
                values: new object[,]
                {
                    { 1, "Passport scan", true },
                    { 2, "CV", true },
                    { 3, "Motivation letter", true },
                    { 4, "School certificates / diplomas", true },
                    { 5, "English test certificate (optional)", false },
                    { 6, "Proof of funds (if needed)", false }
                });

            migrationBuilder.InsertData(
                table: "VisaDocumentChecklists",
                columns: new[] { "Id", "DocumentType", "IsRequired" },
                values: new object[,]
                {
                    { 1, "Passport", true },
                    { 2, "Acceptance letter", true },
                    { 3, "Visa application form", true },
                    { 4, "Proof of accommodation", false },
                    { 5, "Travel insurance", false },
                    { 6, "Bank statements / proof of funds", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ApplicationDocumentChecklists",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "VisaDocumentChecklists",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
