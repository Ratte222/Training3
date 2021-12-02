﻿// <auto-generated />
using System;
using DAL_NS.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL_NS.Migrations
{
    [DbContext(typeof(QueueSystemDbContext))]
    partial class QueueSystemDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DAL_NS.Entity.Credentials", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Login")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NotificationId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("SmtpHost")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId")
                        .IsUnique();

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("DAL_NS.Entity.Notification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("DateTimeCreate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateTimeOfTheLastAttemptToSend")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Header")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("IsSend")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MessageBody")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("NumberOfAttemptToSent")
                        .HasColumnType("int");

                    b.Property<string>("Recipient")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Sender")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("TypeNotification")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("DAL_NS.Entity.NotificationException", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<long?>("HResult")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("NotificationId")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<string>("Type")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId")
                        .IsUnique();

                    b.ToTable("NotificationException");
                });

            modelBuilder.Entity("DAL_NS.Entity.Credentials", b =>
                {
                    b.HasOne("DAL_NS.Entity.Notification", null)
                        .WithOne("Credentials")
                        .HasForeignKey("DAL_NS.Entity.Credentials", "NotificationId");
                });

            modelBuilder.Entity("DAL_NS.Entity.NotificationException", b =>
                {
                    b.HasOne("DAL_NS.Entity.Notification", null)
                        .WithOne("Exception")
                        .HasForeignKey("DAL_NS.Entity.NotificationException", "NotificationId");
                });
#pragma warning restore 612, 618
        }
    }
}