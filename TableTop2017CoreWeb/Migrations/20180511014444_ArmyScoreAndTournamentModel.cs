using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class ArmyScoreAndTournamentModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bye",
                table: "RoundMatchups");

            migrationBuilder.AddColumn<int>(
                name: "ArmyScore",
                table: "Players",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Tournaments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ArmyScoreRatio = table.Column<double>(nullable: false),
                    BattleScoreRatio = table.Column<double>(nullable: false),
                    SportsmanshipScoreRatio = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tournaments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tournaments");

            migrationBuilder.DropColumn(
                name: "ArmyScore",
                table: "Players");

            migrationBuilder.AddColumn<bool>(
                name: "Bye",
                table: "RoundMatchups",
                nullable: false,
                defaultValue: false);
        }
    }
}
