using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class RoundMatchupsUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "battlePoints",
                table: "RoundMatchups");

            migrationBuilder.AddColumn<int>(
                name: "battleScore",
                table: "RoundMatchups",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "battleScore",
                table: "RoundMatchups");

            migrationBuilder.AddColumn<string>(
                name: "battlePoints",
                table: "RoundMatchups",
                nullable: true);
        }
    }
}
