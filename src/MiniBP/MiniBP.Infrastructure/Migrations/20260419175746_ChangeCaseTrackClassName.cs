using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MiniBP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCaseTrackClassName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseTrackers");

            migrationBuilder.CreateTable(
                name: "CaseTracks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    StepTitle = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    FlowStep = table.Column<int>(type: "integer", nullable: false),
                    CurrentUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    PreviousUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    IsLatestTrack = table.Column<bool>(type: "boolean", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTracks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTracks_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseTracks_CaseId",
                table: "CaseTracks",
                column: "CaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseTracks");

            migrationBuilder.CreateTable(
                name: "CaseTrackers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    CaseId = table.Column<int>(type: "integer", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CurrentUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FlowStep = table.Column<int>(type: "integer", nullable: false),
                    IsLatestTrack = table.Column<bool>(type: "boolean", nullable: false),
                    PreviousUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    StepTitle = table.Column<string>(type: "VARCHAR(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseTrackers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CaseTrackers_Cases_CaseId",
                        column: x => x.CaseId,
                        principalTable: "Cases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CaseTrackers_CaseId",
                table: "CaseTrackers",
                column: "CaseId");
        }
    }
}
