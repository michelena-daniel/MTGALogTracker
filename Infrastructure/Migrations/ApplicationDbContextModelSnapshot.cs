﻿// <auto-generated />
using System;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Domain.Entities.Match", b =>
                {
                    b.Property<string>("MatchId")
                        .HasColumnType("text");

                    b.Property<string>("HomeUser")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<bool>("IsDraw")
                        .HasColumnType("boolean");

                    b.Property<string>("MatchCompletedReason")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerOneMtgaId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerOneName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerTwoMtgaId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("PlayerTwoName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RequestId")
                        .HasColumnType("integer");

                    b.Property<string>("TimeStamp")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TransactionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("WinningTeamId")
                        .HasColumnType("integer");

                    b.HasKey("MatchId");

                    b.HasIndex("HomeUser");

                    b.HasIndex("MatchId")
                        .IsUnique();

                    b.ToTable("Matches");
                });

            modelBuilder.Entity("Domain.Entities.PlayerRank", b =>
                {
                    b.Property<string>("LogId")
                        .HasColumnType("text");

                    b.Property<string>("ConstructedClass")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("ConstructedLevel")
                        .HasColumnType("integer");

                    b.Property<int>("ConstructedMatchesDrawn")
                        .HasColumnType("integer");

                    b.Property<int>("ConstructedMatchesLost")
                        .HasColumnType("integer");

                    b.Property<int>("ConstructedMatchesWon")
                        .HasColumnType("integer");

                    b.Property<int>("ConstructedSeasonOrdinal")
                        .HasColumnType("integer");

                    b.Property<int>("ConstructedStep")
                        .HasColumnType("integer");

                    b.Property<string>("LimitedClass")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("LimitedLevel")
                        .HasColumnType("integer");

                    b.Property<int>("LimitedMatchesLost")
                        .HasColumnType("integer");

                    b.Property<int>("LimitedMatchesWon")
                        .HasColumnType("integer");

                    b.Property<int>("LimitedSeasonOrdinal")
                        .HasColumnType("integer");

                    b.Property<int>("LimitedStep")
                        .HasColumnType("integer");

                    b.Property<string>("MtgArenaUserId")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("LogId");

                    b.HasIndex("LogId")
                        .IsUnique();

                    b.HasIndex("MtgArenaUserId");

                    b.HasIndex("TimeStamp");

                    b.ToTable("PlayerRanks");
                });

            modelBuilder.Entity("Domain.Entities.UserInfo", b =>
                {
                    b.Property<string>("MtgaInternalUserId")
                        .HasColumnType("VARCHAR(255)");

                    b.Property<DateTime?>("LastLogin")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserCode")
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("UserNameWithCode")
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("MtgaInternalUserId");

                    b.HasIndex("MtgaInternalUserId")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.Match", b =>
                {
                    b.HasOne("Domain.Entities.UserInfo", "User")
                        .WithMany("Matches")
                        .HasForeignKey("HomeUser")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.PlayerRank", b =>
                {
                    b.HasOne("Domain.Entities.UserInfo", "User")
                        .WithMany("PlayerRanks")
                        .HasForeignKey("MtgArenaUserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserInfo", b =>
                {
                    b.Navigation("Matches");

                    b.Navigation("PlayerRanks");
                });
#pragma warning restore 612, 618
        }
    }
}
