using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class PlayersRoundMatchupsRestructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_currentOpponentid",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_opponentid",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_playerid",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "roundBattleScore",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "table",
                table: "RoundMatchups",
                newName: "Table");

            migrationBuilder.RenameColumn(
                name: "roundNo",
                table: "RoundMatchups",
                newName: "RoundNo");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "RoundMatchups",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sportsmanshipPoints",
                table: "RoundMatchups",
                newName: "PlayerTwoSportsmanshipScore");

            migrationBuilder.RenameColumn(
                name: "playerid",
                table: "RoundMatchups",
                newName: "PlayerTwoId");

            migrationBuilder.RenameColumn(
                name: "opponentid",
                table: "RoundMatchups",
                newName: "PlayerOneId");

            migrationBuilder.RenameColumn(
                name: "battleScore",
                table: "RoundMatchups",
                newName: "PlayerTwoBattleScore");

            migrationBuilder.RenameIndex(
                name: "IX_RoundMatchups_playerid",
                table: "RoundMatchups",
                newName: "IX_RoundMatchups_PlayerTwoId");

            migrationBuilder.RenameIndex(
                name: "IX_RoundMatchups_opponentid",
                table: "RoundMatchups",
                newName: "IX_RoundMatchups_PlayerOneId");

            migrationBuilder.RenameColumn(
                name: "sportsmanshipScore",
                table: "Players",
                newName: "SportsmanshipScore");

            migrationBuilder.RenameColumn(
                name: "notes",
                table: "Players",
                newName: "Notes");

            migrationBuilder.RenameColumn(
                name: "emailAddress",
                table: "Players",
                newName: "EmailAddress");

            migrationBuilder.RenameColumn(
                name: "currentOpponentid",
                table: "Players",
                newName: "CurrentOpponentId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Players",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "totalBattleScore",
                table: "Players",
                newName: "BattleScore");

            migrationBuilder.RenameColumn(
                name: "lastName",
                table: "Players",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "hasPaid",
                table: "Players",
                newName: "Paid");

            migrationBuilder.RenameColumn(
                name: "firstName",
                table: "Players",
                newName: "Army");

            migrationBuilder.RenameIndex(
                name: "IX_Players_currentOpponentid",
                table: "Players",
                newName: "IX_Players_CurrentOpponentId");

            migrationBuilder.AddColumn<int>(
                name: "PlayerOneBattleScore",
                table: "RoundMatchups",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "PlayerOneSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Players",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Players_CurrentOpponentId",
                table: "Players",
                column: "CurrentOpponentId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerOneId",
                table: "RoundMatchups",
                column: "PlayerOneId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_PlayerTwoId",
                table: "RoundMatchups",
                column: "PlayerTwoId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Players_CurrentOpponentId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerOneId",
                table: "RoundMatchups");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Players_PlayerTwoId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerOneBattleScore",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "PlayerOneSportsmanshipScore",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "Players");

            migrationBuilder.RenameColumn(
                name: "Table",
                table: "RoundMatchups",
                newName: "table");

            migrationBuilder.RenameColumn(
                name: "RoundNo",
                table: "RoundMatchups",
                newName: "roundNo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "RoundMatchups",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PlayerTwoSportsmanshipScore",
                table: "RoundMatchups",
                newName: "sportsmanshipPoints");

            migrationBuilder.RenameColumn(
                name: "PlayerTwoId",
                table: "RoundMatchups",
                newName: "playerid");

            migrationBuilder.RenameColumn(
                name: "PlayerTwoBattleScore",
                table: "RoundMatchups",
                newName: "battleScore");

            migrationBuilder.RenameColumn(
                name: "PlayerOneId",
                table: "RoundMatchups",
                newName: "opponentid");

            migrationBuilder.RenameIndex(
                name: "IX_RoundMatchups_PlayerTwoId",
                table: "RoundMatchups",
                newName: "IX_RoundMatchups_playerid");

            migrationBuilder.RenameIndex(
                name: "IX_RoundMatchups_PlayerOneId",
                table: "RoundMatchups",
                newName: "IX_RoundMatchups_opponentid");

            migrationBuilder.RenameColumn(
                name: "SportsmanshipScore",
                table: "Players",
                newName: "sportsmanshipScore");

            migrationBuilder.RenameColumn(
                name: "Notes",
                table: "Players",
                newName: "notes");

            migrationBuilder.RenameColumn(
                name: "EmailAddress",
                table: "Players",
                newName: "emailAddress");

            migrationBuilder.RenameColumn(
                name: "CurrentOpponentId",
                table: "Players",
                newName: "currentOpponentid");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Players",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Paid",
                table: "Players",
                newName: "hasPaid");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Players",
                newName: "lastName");

            migrationBuilder.RenameColumn(
                name: "BattleScore",
                table: "Players",
                newName: "totalBattleScore");

            migrationBuilder.RenameColumn(
                name: "Army",
                table: "Players",
                newName: "firstName");

            migrationBuilder.RenameIndex(
                name: "IX_Players_CurrentOpponentId",
                table: "Players",
                newName: "IX_Players_currentOpponentid");

            migrationBuilder.AddColumn<int>(
                name: "roundBattleScore",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Players_currentOpponentid",
                table: "Players",
                column: "currentOpponentid",
                principalTable: "Players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_opponentid",
                table: "RoundMatchups",
                column: "opponentid",
                principalTable: "Players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Players_playerid",
                table: "RoundMatchups",
                column: "playerid",
                principalTable: "Players",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
