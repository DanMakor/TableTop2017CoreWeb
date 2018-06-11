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
                name: "RoundsModelId",
                table: "RoundMatchups",
                nullable: true);
            
            migrationBuilder.CreateTable(
                name: "RoundsModel",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    NoTableTops = table.Column<int>(nullable: false),
                    RoundNo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoundsModel", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoundMatchups_RoundsModelId",
                table: "RoundMatchups",
                column: "RoundsModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_RoundsModelId",
                table: "Players",
                column: "RoundsModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_RoundsModel_RoundsModelId",
                table: "Players",
                column: "RoundsModelId",
                principalTable: "RoundsModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RoundMatchups_RoundsModel_RoundsModelId",
                table: "RoundMatchups",
                column: "RoundsModelId",
                principalTable: "RoundsModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_RoundsModel_RoundsModelId",
                table: "Players");

            migrationBuilder.DropForeignKey(
                name: "FK_RoundMatchups_RoundsModel_RoundsModelId",
                table: "RoundMatchups");

            migrationBuilder.DropTable(
                name: "RoundsModel");

            migrationBuilder.DropIndex(
                name: "IX_RoundMatchups_RoundsModelId",
                table: "RoundMatchups");

            migrationBuilder.DropIndex(
                name: "IX_Players_RoundsModelId",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "RoundsModelId",
                table: "RoundMatchups");

            migrationBuilder.DropColumn(
                name: "RoundsModelId",
                table: "Players");
        }
    }
}
