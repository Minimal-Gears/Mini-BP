using Microsoft.EntityFrameworkCore.Migrations;

namespace BPMS.Migrations
{
    public partial class _20200204151855 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CaseTrackers_Cases_CaseId1",
                table: "CaseTrackers");

            migrationBuilder.DropIndex(
                name: "IX_CaseTrackers_CaseId1",
                table: "CaseTrackers");

            migrationBuilder.DropColumn(
                name: "CaseId1",
                table: "CaseTrackers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseId1",
                table: "CaseTrackers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaseTrackers_CaseId1",
                table: "CaseTrackers",
                column: "CaseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CaseTrackers_Cases_CaseId1",
                table: "CaseTrackers",
                column: "CaseId1",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
