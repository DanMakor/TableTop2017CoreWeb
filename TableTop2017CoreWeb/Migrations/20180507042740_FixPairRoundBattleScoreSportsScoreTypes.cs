using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class FixPairRoundBattleScoreSportsScoreTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerFourBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerThreeBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerFourBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerThreeBattleScoreId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups");

            migrationBuilder.AddColumn<int>(
                name: "PlayerFourBattleScore",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerFourSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerThreeBattleScore",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PlayerThreeSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerFourBattleScore",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerFourSportsmanshipScore",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerThreeBattleScore",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerThreeSportsmanshipScore",
                table: "RoundMatchups");

            migrationBuilder.AddColumn<int>(
                name: "PlayerFourBattleScoreId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerThreeBattleScoreId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerFourBattleScoreId",
                table: "RoundMatchups",
                column: "PlayerFourBattleScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups",
                column: "PlayerFourSportsmanshipScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerThreeBattleScoreId",
                table: "RoundMatchups",
                column: "PlayerThreeBattleScoreId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups",
                column: "PlayerThreeSportsmanshipScoreId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourBattleScoreId",
                table: "RoundMatchups",
                column: "PlayerFourBattleScoreId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerFourSportsmanshipScoreId",
                table: "RoundMatchups",
                column: "PlayerFourSportsmanshipScoreId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeBattleScoreId",
                table: "RoundMatchups",
                column: "PlayerThreeBattleScoreId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerThreeSportsmanshipScoreId",
                table: "RoundMatchups",
                column: "PlayerThreeSportsmanshipScoreId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
