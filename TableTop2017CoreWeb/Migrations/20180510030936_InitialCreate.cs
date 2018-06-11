using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Active = table.Column<bool>(nullable: false),
                    Army = table.Column<string>(nullable: true),
                    BattleScore = table.Column<int>(nullable: false),
                    CurrentOpponentId = table.Column<int>(nullable: true),
                    EmailAddress = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    Paid = table.Column<bool>(nullable: false),
                    SportsmanshipScore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Players_CurrentOpponentId",
                        column: x => x.CurrentOpponentId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoundNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoundMatchups",
                columns: table => new
                {
                    PlayerFourBattleScore = table.Column<int>(nullable: true),
                    PlayerFourId = table.Column<int>(nullable: true),
                    PlayerFourSportsmanshipScore = table.Column<int>(nullable: true),
                    PlayerThreeBattleScore = table.Column<int>(nullable: true),
                    PlayerThreeId = table.Column<int>(nullable: true),
                    PlayerThreeSportsmanshipScore = table.Column<int>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Discriminator = table.Column<string>(nullable: false),
                    PlayerOneBattleScore = table.Column<int>(nullable: false),
                    PlayerOneId = table.Column<int>(nullable: true),
                    PlayerOneSportsmanshipScore = table.Column<int>(nullable: false),
                    PlayerTwoBattleScore = table.Column<int>(nullable: false),
                    PlayerTwoId = table.Column<int>(nullable: true),
                    PlayerTwoSportsmanshipScore = table.Column<int>(nullable: false),
                    RoundId = table.Column<int>(nullable: true),
                    RoundNo = table.Column<int>(nullable: false),
                    Table = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundMatchups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_PlayerFourId",
                        column: x => x.PlayerFourId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_PlayerThreeId",
                        column: x => x.PlayerThreeId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_PlayerOneId",
                        column: x => x.PlayerOneId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_PlayerTwoId",
                        column: x => x.PlayerTwoId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Rounds_RoundId",
                        column: x => x.RoundId,
                        principalTable: "Rounds",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_CurrentOpponentId",
                table: "Players",
                column: "CurrentOpponentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerFourId",
                table: "RoundMatchups",
                column: "PlayerFourId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerThreeId",
                table: "RoundMatchups",
                column: "PlayerThreeId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerOneId",
                table: "RoundMatchups",
                column: "PlayerOneId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_PlayerTwoId",
                table: "RoundMatchups",
                column: "PlayerTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_RoundId",
                table: "RoundMatchups",
                column: "RoundId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoundMatchups");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Rounds");
        }
    }
}
