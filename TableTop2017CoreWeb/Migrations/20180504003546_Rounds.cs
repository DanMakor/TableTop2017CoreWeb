using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace TableTop2017CoreWeb.Migrations
{
    public partial class Rounds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoundId",
                table: "RoundMatchups",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Rounds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoundNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rounds", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_RoundId",
                table: "RoundMatchups",
                column: "RoundId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_Rounds_RoundId",
                table: "RoundMatchups",
                column: "RoundId",
                principalTable: "Rounds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_Rounds_RoundId",
                table: "RoundMatchups");

            migrationBuilder.DropTable(
                name: "Rounds");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_RoundId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "RoundId",
                table: "RoundMatchups");
        }
    }
}
