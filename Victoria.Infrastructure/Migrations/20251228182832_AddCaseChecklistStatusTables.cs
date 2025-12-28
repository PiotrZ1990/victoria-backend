using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCaseChecklistStatusTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CaseApplicationChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    ChecklistId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseApplicationChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseApplicationChecklistItems_ApplicationDocumentChecklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "ApplicationDocumentChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaseApplicationChecklistItems_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseApplicationChecklistItems_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CaseVisaChecklistItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    ChecklistId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseVisaChecklistItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseVisaChecklistItems_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseVisaChecklistItems_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CaseVisaChecklistItems_VisaDocumentChecklists_ChecklistId",
                        column: x => x.ChecklistId,
                        principalTable: "VisaDocumentChecklists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseApplicationChecklistItems_CaseFileId_ChecklistId",
                table: "CaseApplicationChecklistItems",
                columns: new[] { "CaseFileId", "ChecklistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseApplicationChecklistItems_ChecklistId",
                table: "CaseApplicationChecklistItems",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseApplicationChecklistItems_DocumentId",
                table: "CaseApplicationChecklistItems",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseVisaChecklistItems_CaseFileId_ChecklistId",
                table: "CaseVisaChecklistItems",
                columns: new[] { "CaseFileId", "ChecklistId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseVisaChecklistItems_ChecklistId",
                table: "CaseVisaChecklistItems",
                column: "ChecklistId");

            migrationBuilder.CreateIndex(
                name: "IX_CaseVisaChecklistItems_DocumentId",
                table: "CaseVisaChecklistItems",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseApplicationChecklistItems");

            migrationBuilder.DropTable(
                name: "CaseVisaChecklistItems");
        }
    }
}
