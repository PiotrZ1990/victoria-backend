using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Victoria.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVisaAndAccommodation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccommodationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BudgetMin = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BudgetMax = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationRequests_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VisaApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CaseFileId = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VisaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisaApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VisaApplications_CaseFiles_CaseFileId",
                        column: x => x.CaseFileId,
                        principalTable: "CaseFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccommodationBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccommodationRequestId = table.Column<int>(type: "int", nullable: false),
                    ProviderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoveInDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MoveOutDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MonthlyPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccommodationBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccommodationBookings_AccommodationRequests_AccommodationRequestId",
                        column: x => x.AccommodationRequestId,
                        principalTable: "AccommodationRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationBookings_AccommodationRequestId",
                table: "AccommodationBookings",
                column: "AccommodationRequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationRequests_CaseFileId",
                table: "AccommodationRequests",
                column: "CaseFileId");

            migrationBuilder.CreateIndex(
                name: "IX_AccommodationRequests_Status",
                table: "AccommodationRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VisaApplications_CaseFileId",
                table: "VisaApplications",
                column: "CaseFileId");

            migrationBuilder.CreateIndex(
                name: "IX_VisaApplications_Status",
                table: "VisaApplications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccommodationBookings");

            migrationBuilder.DropTable(
                name: "VisaApplications");

            migrationBuilder.DropTable(
                name: "AccommodationRequests");
        }
    }
}
