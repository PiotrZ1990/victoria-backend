using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCrmCasesApplications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestedCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InterestedService = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CaseFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    CaseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Stage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseFiles_Leads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "Leads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudyApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UniversityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProgramName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyApplications_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_CaseNumber",
                table: "CaseFiles",
                column: "CaseNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseFiles_LeadId",
                table: "CaseFiles",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_Email",
                table: "Leads",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_StudyApplications_CaseFileId",
                table: "StudyApplications",
                column: "CaseFileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudyApplications");

            migrationBuilder.DropTable(
                name: "CaseFiles");

            migrationBuilder.DropTable(
                name: "Leads");
        }
    }
}
