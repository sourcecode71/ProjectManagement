using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistance.Migrations
{
    public partial class ProPoposal3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Proposals_ProposalsId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProposalsId",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProposalsId",
                table: "Projects",
                column: "ProposalsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Proposals_ProposalsId",
                table: "Projects",
                column: "ProposalsId",
                principalTable: "Proposals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
