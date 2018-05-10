using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class RemoveCurrentTeammate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_CurrentTeammateId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_CurrentTeammateId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "CurrentTeammateId",
                table: "Players");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurrentTeammateId",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Players_CurrentTeammateId",
                table: "Players",
                column: "CurrentTeammateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Players_CurrentTeammateId",
                table: "Players",
                column: "CurrentTeammateId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
