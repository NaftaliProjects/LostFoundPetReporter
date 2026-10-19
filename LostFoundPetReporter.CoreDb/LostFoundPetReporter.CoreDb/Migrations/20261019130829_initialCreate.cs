using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LostFoundPetReporter.CoreDb.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HashedPassword = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DateReported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    PetDescription_Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Breed = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Colors = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Sex = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Age = table.Column<double>(type: "float", nullable: true),
                    PetDescription_Size = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_WeightKg = table.Column<double>(type: "float", nullable: true),
                    PetDescription_CoatLength = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_CoatType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_Pattern = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_DistinctiveMarkings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_EyeColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_EarDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_TailDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_CollarPresent = table.Column<bool>(type: "bit", nullable: true),
                    PetDescription_CollarColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_CollarType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetDescription_HarnessPresent = table.Column<bool>(type: "bit", nullable: true),
                    PetDescription_HarnessColor = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Platform = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDevices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Coordinate",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coordinate", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Coordinate_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExtFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExtFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExtFiles_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LostFoundMatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LostReportId = table.Column<int>(type: "int", nullable: false),
                    FoundReportId = table.Column<int>(type: "int", nullable: false),
                    Score = table.Column<double>(type: "float", nullable: false),
                    MatchReason = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostFoundMatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LostFoundMatches_Reports_FoundReportId",
                        column: x => x.FoundReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LostFoundMatches_Reports_LostReportId",
                        column: x => x.LostReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExtFiles_ReportId",
                table: "ExtFiles",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LostFoundMatches_FoundReportId",
                table: "LostFoundMatches",
                column: "FoundReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LostFoundMatches_LostReportId",
                table: "LostFoundMatches",
                column: "LostReportId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_UserId",
                table: "Reports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_Token",
                table: "UserDevices",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_UserId",
                table: "UserDevices",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coordinate");

            migrationBuilder.DropTable(
                name: "ExtFiles");

            migrationBuilder.DropTable(
                name: "LostFoundMatches");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
