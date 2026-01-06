using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccommodationChecklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccommodationDocumentChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationDocumentChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseAccommodationChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    ChecklistId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseAccommodationChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseAccommodationChecklistItems_AccommodationDocumentChecklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "AccommodationDocumentChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationDocumentChecklists_DocumentType",
                table: "AccommodationDocumentChecklists",
                column: "DocumentType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseAccommodationChecklistItems_CaseFileId_ChecklistId",
                table: "CaseAccommodationChecklistItems",
                columns: new[] { "CaseFileId", "ChecklistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseAccommodationChecklistItems_ChecklistId",
                table: "CaseAccommodationChecklistItems",
                column: "ChecklistId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseAccommodationChecklistItems");

            migrationBuilder.DropTable(
                name: "AccommodationDocumentChecklists");
        }
    }
}
