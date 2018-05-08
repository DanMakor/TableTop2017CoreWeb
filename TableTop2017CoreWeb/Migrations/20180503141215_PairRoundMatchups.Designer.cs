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
    [Migration("20180503141215_PairRoundMatchups")]
    partial class PairRoundMatchups
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

                    b.Property<int?>("CurrentTeammateId");

                    b.Property<string>("EmailAddress");

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<bool>("Paid");

                    b.Property<int>("SportsmanshipScore");

                    b.HasKey("Id");

                    b.HasIndex("CurrentOpponentId");

                    b.HasIndex("CurrentTeammateId");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.RoundMatchups", b =>
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

                    b.Property<int>("RoundNo");

                    b.Property<int>("Table");

                    b.HasKey("Id");

                    b.HasIndex("PlayerOneId");

                    b.HasIndex("PlayerTwoId");

                    b.ToTable("RoundMatchups");

                    b.HasDiscriminator<string>("Discriminator").HasValue("RoundMatchups");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.PairRoundMatchup", b =>
                {
                    b.HasBaseType("TableTop2017CoreWeb.Models.RoundMatchups");

                    b.Property<int?>("PlayerFourId");

                    b.Property<int?>("PlayerThreeId");

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

                    b.HasOne("TableTop2017CoreWeb.Models.Player", "CurrentTeammate")
                        .WithMany()
                        .HasForeignKey("CurrentTeammateId");
                });

            modelBuilder.Entity("TableTop2017CoreWeb.Models.RoundMatchups", b =>
                {
                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerOne")
                        .WithMany()
                        .HasForeignKey("PlayerOneId");

                    b.HasOne("TableTop2017CoreWeb.Models.Player", "PlayerTwo")
                        .WithMany()
                        .HasForeignKey("PlayerTwoId");
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
