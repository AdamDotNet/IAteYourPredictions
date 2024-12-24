﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TryingTwitchOAuth.Data;

#nullable disable

namespace TryingTwitchOAuth.Migrations
{
    [DbContext(typeof(PredictionsDbContext))]
    [Migration("20241224014345_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("TryingTwitchOAuth.Data.Prediction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Conference")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EntriesAreOpen")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsOpen")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ModifiedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Predictions", (string)null);
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.PredictionEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CastTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCorrect")
                        .HasColumnType("INTEGER");

                    b.Property<int>("OptionId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PredictionId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TwitchDisplayName")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitchUid")
                        .HasColumnType("TEXT");

                    b.Property<string>("TwitchUserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.HasIndex("PredictionId");

                    b.ToTable("PredictionEntries");
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.PredictionOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("DisplayText")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsWinner")
                        .HasColumnType("INTEGER");

                    b.Property<int>("PredictionId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("PredictionId");

                    b.ToTable("PredictionOption");
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.PredictionEntry", b =>
                {
                    b.HasOne("TryingTwitchOAuth.Data.PredictionOption", "Option")
                        .WithMany("Entries")
                        .HasForeignKey("OptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("TryingTwitchOAuth.Data.Prediction", "Prediction")
                        .WithMany("Entries")
                        .HasForeignKey("PredictionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Option");

                    b.Navigation("Prediction");
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.PredictionOption", b =>
                {
                    b.HasOne("TryingTwitchOAuth.Data.Prediction", "Prediction")
                        .WithMany("Options")
                        .HasForeignKey("PredictionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Prediction");
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.Prediction", b =>
                {
                    b.Navigation("Entries");

                    b.Navigation("Options");
                });

            modelBuilder.Entity("TryingTwitchOAuth.Data.PredictionOption", b =>
                {
                    b.Navigation("Entries");
                });
#pragma warning restore 612, 618
        }
    }
}
