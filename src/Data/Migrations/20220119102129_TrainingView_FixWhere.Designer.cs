﻿// <auto-generated />
using System;
using Ivao.It.DiscordBot.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Ivao.It.DiscordBot.Data.Migrations
{
    [DbContext(typeof(DiscordDbContext))]
    [Migration("20220119102129_TrainingView_FixWhere")]
    partial class TrainingView_FixWhere
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Ivao.It.DiscordBot.Data.Entities.Exam", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Airport")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("varchar(4)");

                    b.Property<string>("Facility")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateOnly>("PlannedDate")
                        .HasColumnType("date")
                        .HasColumnName("planned_date");

                    b.Property<TimeOnly>("PlannedTime")
                        .HasColumnType("time(6)")
                        .HasColumnName("planned_hour");

                    b.Property<int>("Rating")
                        .HasColumnType("int")
                        .HasColumnName("rating");

                    b.HasKey("Id");

                    b.ToView("examlist");

                    b.HasAnnotation("ReadOnly", "ReadOnly");
                });

            modelBuilder.Entity("Ivao.It.DiscordBot.Data.Entities.Training", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Airport")
                        .IsRequired()
                        .HasMaxLength(4)
                        .HasColumnType("varchar(4)");

                    b.Property<string>("Facility")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateOnly>("PlannedDate")
                        .HasColumnType("date")
                        .HasColumnName("planned_date");

                    b.Property<TimeOnly>("PlannedTime")
                        .HasColumnType("time(6)")
                        .HasColumnName("planned_hour");

                    b.Property<int>("Rating")
                        .HasColumnType("int")
                        .HasColumnName("rating");

                    b.HasKey("Id");

                    b.ToView("trainingslist");

                    b.HasAnnotation("ReadOnly", "ReadOnly");
                });

            modelBuilder.Entity("Ivao.It.DiscordBot.Data.Entities.User", b =>
                {
                    b.Property<string>("Vid")
                        .HasMaxLength(6)
                        .HasColumnType("varchar(6)");

                    b.Property<ulong?>("DiscordUserId")
                        .HasColumnType("bigint unsigned");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("varchar(70)");

                    b.Property<DateTime?>("GdprAcceptDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(70)
                        .HasColumnType("varchar(70)");

                    b.HasKey("Vid");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}
