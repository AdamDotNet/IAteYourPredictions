using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TryingTwitchOAuth.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Predictions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Conference = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: true),
                    IsOpen = table.Column<bool>(type: "INTEGER", nullable: false),
                    EntriesAreOpen = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predictions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PredictionOption",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DisplayText = table.Column<string>(type: "TEXT", nullable: true),
                    IsWinner = table.Column<bool>(type: "INTEGER", nullable: false),
                    PredictionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionOption", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionOption_Predictions_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PredictionEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TwitchUid = table.Column<string>(type: "TEXT", nullable: true),
                    TwitchUserName = table.Column<string>(type: "TEXT", nullable: true),
                    TwitchDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    CastTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCorrect = table.Column<bool>(type: "INTEGER", nullable: false),
                    OptionId = table.Column<int>(type: "INTEGER", nullable: false),
                    PredictionId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PredictionEntries_PredictionOption_OptionId",
                        column: x => x.OptionId,
                        principalTable: "PredictionOption",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PredictionEntries_Predictions_PredictionId",
                        column: x => x.PredictionId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PredictionEntries_OptionId",
                table: "PredictionEntries",
                column: "OptionId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionEntries_PredictionId",
                table: "PredictionEntries",
                column: "PredictionId");

            migrationBuilder.CreateIndex(
                name: "IX_PredictionOption_PredictionId",
                table: "PredictionOption",
                column: "PredictionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredictionEntries");

            migrationBuilder.DropTable(
                name: "PredictionOption");

            migrationBuilder.DropTable(
                name: "Predictions");
        }
    }
}
