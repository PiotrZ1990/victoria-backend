using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedDemoDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "FileResources",
                columns: new[] { "Id", "ContentType", "FileName", "FilePath", "FileSize", "UploadedAt" },
                values: new object[,]
                {
                    { 1, "application/pdf", "passport-scan-demo.pdf", "seed/passport-scan-demo.pdf", 123456L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "application/pdf", "acceptance-letter-demo.pdf", "seed/acceptance-letter-demo.pdf", 234567L, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Documents",
                columns: new[] { "Id", "CreatedAt", "DocumentType", "FileResourceId", "Status" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Passport scan (DEMO)", 1, "Uploaded" },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Acceptance letter (DEMO)", 2, "Uploaded" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Documents",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "FileResources",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "FileResources",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
