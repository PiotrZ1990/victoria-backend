using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentsAndFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApplicationDocumentChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocumentChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VisaDocumentChecklists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRequired = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaDocumentChecklists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileResourceId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_FileResources_FileResourceId",
                        column: x => x.FileResourceId,
                        principalTable: "FileResources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudyApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationDocuments_StudyApplications_StudyApplicationId",
                        column: x => x.StudyApplicationId,
                        principalTable: "StudyApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisaDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisaApplicationId = table.Column<int>(type: "int", nullable: false),
                    DocumentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisaDocuments_Documents_DocumentId",
                        column: x => x.DocumentId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VisaDocuments_VisaApplications_VisaApplicationId",
                        column: x => x.VisaApplicationId,
                        principalTable: "VisaApplications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_DocumentId",
                table: "ApplicationDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationDocuments_StudyApplicationId",
                table: "ApplicationDocuments",
                column: "StudyApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_FileResourceId",
                table: "Documents",
                column: "FileResourceId");

            migrationBuilder.CreateIndex(
                name: "IX_VisaDocuments_DocumentId",
                table: "VisaDocuments",
                column: "DocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_VisaDocuments_VisaApplicationId",
                table: "VisaDocuments",
                column: "VisaApplicationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationDocumentChecklists");

            migrationBuilder.DropTable(
                name: "ApplicationDocuments");

            migrationBuilder.DropTable(
                name: "VisaDocumentChecklists");

            migrationBuilder.DropTable(
                name: "VisaDocuments");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "FileResources");
        }
    }
}
