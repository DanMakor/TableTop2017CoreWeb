using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class PairRoundMatchups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlayerFourId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerThreeId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "RoundMatchups",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CurrentTeammateId",
                table: "Players",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerFourId",
                table: "RoundMatchups",
                column: "PlayerFourId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerThreeId",
                table: "RoundMatchups",
                column: "PlayerThreeId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourId",
                table: "RoundMatchups",
                column: "PlayerFourId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeId",
                table: "RoundMatchups",
                column: "PlayerThreeId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_CurrentTeammateId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourId",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerFourId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerThreeId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_Players_CurrentTeammateId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerFourId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerThreeId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "CurrentTeammateId",
                table: "Players");
        }
    }
}
