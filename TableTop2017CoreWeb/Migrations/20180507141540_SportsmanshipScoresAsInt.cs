using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class SportsmanshipScoresAsInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "PlayerTwoSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerOneSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerThreeSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlayerFourSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlayerTwoSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "PlayerOneSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "PlayerThreeSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerFourSportsmanshipScore",
                table: "RoundMatchups",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
