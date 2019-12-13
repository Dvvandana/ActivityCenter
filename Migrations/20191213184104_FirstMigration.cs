using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ActivityCenter.Migrations
{
    public partial class FirstMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Happenings",
                columns: table => new
                {
                    HappeningId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<int>(nullable: false),
                    HappeningDate = table.Column<DateTime>(nullable: false),
                    StartTime = table.Column<DateTime>(nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    DurationMetric = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false),
                    CreatedById = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Happenings", x => x.HappeningId);
                    table.ForeignKey(
                        name: "FK_Happenings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participations",
                columns: table => new
                {
                    ParticipationId = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParticipantId = table.Column<int>(nullable: false),
                    JoiningActivityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participations", x => x.ParticipationId);
                    table.ForeignKey(
                        name: "FK_Participations_Happenings_JoiningActivityId",
                        column: x => x.JoiningActivityId,
                        principalTable: "Happenings",
                        principalColumn: "HappeningId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participations_Users_ParticipantId",
                        column: x => x.ParticipantId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Happenings_CreatedById",
                table: "Happenings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_JoiningActivityId",
                table: "Participations",
                column: "JoiningActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Participations_ParticipantId",
                table: "Participations",
                column: "ParticipantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Participations");

            migrationBuilder.DropTable(
                name: "Happenings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
