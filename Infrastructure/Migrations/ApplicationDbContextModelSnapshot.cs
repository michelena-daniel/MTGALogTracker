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

            modelBuilder.Entity("Domain.Entities.PlayerRank", b =>
                {
                    b.Property<int>("RankId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("RankId"));

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

                    b.Property<string>("CurrentUser")
                        .IsRequired()
                        .HasColumnType("text");

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

                    b.Property<string>("LogId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("RankId");

                    b.HasIndex("LogId")
                        .IsUnique();

                    b.HasIndex("TimeStamp");

                    b.HasIndex("UserId");

                    b.ToTable("PlayerRanks");
                });

            modelBuilder.Entity("Domain.Entities.UserInfo", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("UserId"));

                    b.Property<string>("UserCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.Property<string>("UserNameWithCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR(100)");

                    b.HasKey("UserId");

                    b.HasIndex("UserNameWithCode")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Domain.Entities.PlayerRank", b =>
                {
                    b.HasOne("Domain.Entities.UserInfo", "User")
                        .WithMany("PlayerRanks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.UserInfo", b =>
                {
                    b.Navigation("PlayerRanks");
                });
#pragma warning restore 612, 618
        }
    }
}
