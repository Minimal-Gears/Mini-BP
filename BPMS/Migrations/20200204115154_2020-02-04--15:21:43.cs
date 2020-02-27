using Microsoft.EntityFrameworkCore.Migrations;

namespace BPMS.Migrations
{
    public partial class _20200204152143 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowParameters_Cases_CaseId1",
                table: "FlowParameters");

            migrationBuilder.DropIndex(
                name: "IX_FlowParameters_CaseId1",
                table: "FlowParameters");

            migrationBuilder.DropColumn(
                name: "CaseId1",
                table: "FlowParameters");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CaseId1",
                table: "FlowParameters",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FlowParameters_CaseId1",
                table: "FlowParameters",
                column: "CaseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowParameters_Cases_CaseId1",
                table: "FlowParameters",
                column: "CaseId1",
                principalTable: "Cases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
