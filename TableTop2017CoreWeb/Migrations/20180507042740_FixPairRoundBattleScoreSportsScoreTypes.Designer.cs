﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using TableTop2017CoreWeb.Data;

namespace TableTop2017CoreWeb.Migrations
{
    [DbContext(typeof(TournamentDbContext))]
    [Migration("20180507042740_FixPairRoundBattleScoreSportsScoreTypes")]
    partial class FixPairRoundBattleScoreSportsScoreTypes
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Army");

                    b.Property<int>("BattleScore");

                    b.Property<int?>("CurrentOpponentId");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<bool>("Paid");

                    b.Property<int>("SportsmanshipScore");

                    b.HasKey("Id");

                    b.HasIndex("CurrentOpponentId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("RoundNo");

                    b.HasKey("Id");

                    b.ToTable("Rounds");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.RoundMatchup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<int>("PlayerOneBattleScore");

                    b.Property<int?>("PlayerOneId");

                    b.Property<string>("PlayerOneSportsmanshipScore");

                    b.Property<int>("PlayerTwoBattleScore");

                    b.Property<int?>("PlayerTwoId");

                    b.Property<string>("PlayerTwoSportsmanshipScore");

                    b.Property<int?>("RoundId");

                    b.Property<int>("RoundNo");

                    b.Property<int>("Table");

                    b.HasKey("Id");

                    b.HasIndex("PlayerOneId");

                    b.HasIndex("PlayerTwoId");

                    b.HasIndex("RoundId");

                    b.ToTable("RoundMatchups");

                    b.HasDiscriminator<string>("Discriminator").HasValue("RoundMatchup");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.PairRoundMatchup", b =>
                {
                    b.HasBaseType("TableTop2017CoreWeb.Models.RoundMatchup");

                    b.Property<int>("PlayerFourBattleScore");

                    b.Property<int?>("PlayerFourId");

                    b.Property<string>("PlayerFourSportsmanshipScore");

                    b.Property<int>("PlayerThreeBattleScore");

                    b.Property<int?>("PlayerThreeId");

                    b.Property<string>("PlayerThreeSportsmanshipScore");

                    b.HasIndex("PlayerFourId");

                    b.HasIndex("PlayerThreeId");

                    b.ToTable("PairRoundMatchup");

                    b.HasDiscriminator().HasValue("PairRoundMatchup");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Player", b =>
                {
                    b.HasOne("TableTop2017CoreWeb.Models.Player", "CurrentOpponent")
                        .WithMany()
                        .HasForeignKey("CurrentOpponentId");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.RoundMatchup", b =>
                {
                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerOne")
                        .WithMany()
                        .HasForeignKey("PlayerOneId");

                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerTwo")
                        .WithMany()
                        .HasForeignKey("PlayerTwoId");

                    b.HasOne("TableTop2017CoreWeb.Models.Round")
                        .WithMany("RoundMatchups")
                        .HasForeignKey("RoundId");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.PairRoundMatchup", b =>
                {
                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerFour")
                        .WithMany()
                        .HasForeignKey("PlayerFourId");

                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerThree")
                        .WithMany()
                        .HasForeignKey("PlayerThreeId");
                });
#pragma warning restore 612, 618
        }
    }
}
