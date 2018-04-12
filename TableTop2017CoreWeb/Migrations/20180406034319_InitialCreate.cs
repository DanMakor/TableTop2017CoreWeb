using Microsoft.EntityFrameworkCore.Metadata;
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
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    currentOpponentid = table.Column<int>(nullable: true),
                    emailAddress = table.Column<string>(nullable: true),
                    firstName = table.Column<string>(nullable: true),
                    hasPaid = table.Column<bool>(nullable: false),
                    lastName = table.Column<string>(nullable: true),
                    notes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.id);
                    table.ForeignKey(
                        name: "FK_Players_Players_currentOpponentid",
                        column: x => x.currentOpponentid,
                        principalTable: "Players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RoundMatchups",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    battlePoints = table.Column<string>(nullable: true),
                    opponentid = table.Column<int>(nullable: true),
                    playerid = table.Column<int>(nullable: true),
                    roundNo = table.Column<int>(nullable: false),
                    sportsmanshipPoints = table.Column<string>(nullable: true),
                    table = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundMatchups", x => x.id);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_opponentid",
                        column: x => x.opponentid,
                        principalTable: "Players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoundMatchups_Players_playerid",
                        column: x => x.playerid,
                        principalTable: "Players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_currentOpponentid",
                table: "Players",
                column: "currentOpponentid");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_opponentid",
                table: "RoundMatchups",
                column: "opponentid");

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_playerid",
                table: "RoundMatchups",
                column: "playerid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoundMatchups");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
