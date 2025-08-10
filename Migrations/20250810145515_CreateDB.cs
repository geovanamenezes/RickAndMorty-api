using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RickAndMorty.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FileData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    TotalLocations = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalFemaleCharacters = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalMaleCharacters = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGenderlessCharacters = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalGenderUnknownCharacters = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadeFilePath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Location",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Dimension = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Location", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UploadHistory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Episode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    AirDate = table.Column<string>(type: "TEXT", nullable: false),
                    Episode = table.Column<string>(type: "TEXT", nullable: false),
                    FileDataModelId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episode_FileData_FileDataModelId",
                        column: x => x.FileDataModelId,
                        principalTable: "FileData",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Character",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    Species = table.Column<string>(type: "TEXT", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Gender = table.Column<string>(type: "TEXT", nullable: false),
                    OriginId = table.Column<int>(type: "INTEGER", nullable: true),
                    LocationId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Character", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Character_Location_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Character_Location_OriginId",
                        column: x => x.OriginId,
                        principalTable: "Location",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileDataEpisode",
                columns: table => new
                {
                    FileDataId = table.Column<string>(type: "TEXT", nullable: false),
                    EpisodeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDataEpisode", x => new { x.FileDataId, x.EpisodeId });
                    table.ForeignKey(
                        name: "FK_FileDataEpisode_Episode_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FileDataEpisode_FileData_FileDataId",
                        column: x => x.FileDataId,
                        principalTable: "FileData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CharacterEpisode",
                columns: table => new
                {
                    CharacterId = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterEpisode", x => new { x.CharacterId, x.EpisodeId });
                    table.ForeignKey(
                        name: "FK_CharacterEpisode_Character_CharacterId",
                        column: x => x.CharacterId,
                        principalTable: "Character",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterEpisode_Episode_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Character_LocationId",
                table: "Character",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Character_OriginId",
                table: "Character",
                column: "OriginId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEpisode_CharacterId",
                table: "CharacterEpisode",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterEpisode_EpisodeId",
                table: "CharacterEpisode",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Episode_FileDataModelId",
                table: "Episode",
                column: "FileDataModelId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDataEpisode_EpisodeId",
                table: "FileDataEpisode",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileDataEpisode_FileDataId",
                table: "FileDataEpisode",
                column: "FileDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterEpisode");

            migrationBuilder.DropTable(
                name: "FileDataEpisode");

            migrationBuilder.DropTable(
                name: "UploadHistory");

            migrationBuilder.DropTable(
                name: "Character");

            migrationBuilder.DropTable(
                name: "Episode");

            migrationBuilder.DropTable(
                name: "Location");

            migrationBuilder.DropTable(
                name: "FileData");
        }
    }
}
