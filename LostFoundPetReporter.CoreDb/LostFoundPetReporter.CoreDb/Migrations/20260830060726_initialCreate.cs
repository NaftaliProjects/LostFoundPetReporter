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
                    HashedPassword = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FoundReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_FoundReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LostReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
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
                    table.PrimaryKey("PK_LostReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LostReports_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoundCoordinate",
                columns: table => new
                {
                    FoundReportId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundCoordinate", x => x.FoundReportId);
                    table.ForeignKey(
                        name: "FK_FoundCoordinate_FoundReports_FoundReportId",
                        column: x => x.FoundReportId,
                        principalTable: "FoundReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FoundReportExtFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FoundReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundReportExtFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundReportExtFiles_FoundReports_FoundReportId",
                        column: x => x.FoundReportId,
                        principalTable: "FoundReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LostCoordinate",
                columns: table => new
                {
                    LostReportId = table.Column<int>(type: "int", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostCoordinate", x => x.LostReportId);
                    table.ForeignKey(
                        name: "FK_LostCoordinate_LostReports_LostReportId",
                        column: x => x.LostReportId,
                        principalTable: "LostReports",
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
                        name: "FK_LostFoundMatches_FoundReports_FoundReportId",
                        column: x => x.FoundReportId,
                        principalTable: "FoundReports",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LostFoundMatches_LostReports_LostReportId",
                        column: x => x.LostReportId,
                        principalTable: "LostReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LostReportExtFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FilePath = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    LostReportId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LostReportExtFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LostReportExtFiles_LostReports_LostReportId",
                        column: x => x.LostReportId,
                        principalTable: "LostReports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoundReportExtFiles_FoundReportId",
                table: "FoundReportExtFiles",
                column: "FoundReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundReports_UserId",
                table: "FoundReports",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LostFoundMatches_FoundReportId",
                table: "LostFoundMatches",
                column: "FoundReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LostFoundMatches_LostReportId_FoundReportId",
                table: "LostFoundMatches",
                columns: new[] { "LostReportId", "FoundReportId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LostReportExtFiles_LostReportId",
                table: "LostReportExtFiles",
                column: "LostReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LostReports_UserId",
                table: "LostReports",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundCoordinate");

            migrationBuilder.DropTable(
                name: "FoundReportExtFiles");

            migrationBuilder.DropTable(
                name: "LostCoordinate");

            migrationBuilder.DropTable(
                name: "LostFoundMatches");

            migrationBuilder.DropTable(
                name: "LostReportExtFiles");

            migrationBuilder.DropTable(
                name: "FoundReports");

            migrationBuilder.DropTable(
                name: "LostReports");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
