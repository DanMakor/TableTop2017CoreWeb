﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System;
using TableTop2017CoreWeb.Data;

namespace TableTop2017CoreWeb.Migrations
{
    [DbContext(typeof(TournamentDbContext))]
    partial class TournamentDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.3-rtm-10026");

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Army");

                    b.Property<int>("ArmyScore");

                    b.Property<int>("BattleScore");

                    b.Property<bool>("Bye");

                    b.Property<int?>("CurrentOpponentId");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<bool>("Paid");

                    b.Property<int?>("RoundId");

                    b.Property<int>("SportsmanshipScore");

                    b.Property<int>("WeightedScore");

                    b.HasKey("Id");

                    b.HasIndex("CurrentOpponentId");

                    b.HasIndex("RoundId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Round", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("NoTableTops");

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

                    b.Property<int>("PlayerOneSportsmanshipScore");

                    b.Property<int>("PlayerTwoBattleScore");

                    b.Property<int?>("PlayerTwoId");

                    b.Property<int>("PlayerTwoSportsmanshipScore");

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

            modelBuilder.Entity("TableTop2017CoreWeb.Models.Tournament", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("ArmyScoreRatio");

                    b.Property<double>("BattleScoreRatio");

                    b.Property<double>("SportsmanshipScoreRatio");

                    b.HasKey("Id");

                    b.ToTable("Tournaments");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.PairRoundMatchup", b =>
                {
                    b.HasBaseType("TableTop2017CoreWeb.Models.RoundMatchup");

                    b.Property<int>("PlayerFourBattleScore");

                    b.Property<int?>("PlayerFourId");

                    b.Property<int>("PlayerFourSportsmanshipScore");

                    b.Property<int>("PlayerThreeBattleScore");

                    b.Property<int?>("PlayerThreeId");

                    b.Property<int>("PlayerThreeSportsmanshipScore");

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

                    b.HasOne("TableTop2017CoreWeb.Models.Round")
                        .WithMany("Players")
                        .HasForeignKey("RoundId");
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
