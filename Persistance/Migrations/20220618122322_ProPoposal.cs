using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistance.Migrations
{
    public partial class ProPoposal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ProposalId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalsId",
                table: "Projects",
                type: "uniqueidentifier",
                nullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Proposals_ProposalsId",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProposalsId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProposalId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProposalsId",
                table: "Projects");
        }
    }
}
