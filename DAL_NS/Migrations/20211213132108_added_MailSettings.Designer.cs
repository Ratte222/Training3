﻿// <auto-generated />
using System;
using DAL_NS.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL_NS.Migrations
{
    [DbContext(typeof(QueueSystemDbContext))]
    [Migration("20211213132108_added_MailSettings")]
    partial class added_MailSettings
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.12");

            modelBuilder.Entity("DAL_NS.Entity.Credentials", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Login")
                        .HasColumnType("longtext");

                    b.Property<string>("NotificationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Password")
                        .HasColumnType("longtext");

                    b.Property<string>("SmtpHost")
                        .HasColumnType("longtext");

                    b.Property<int>("SmtpPort")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId")
                        .IsUnique();

                    b.ToTable("Credentials");
                });

            modelBuilder.Entity("DAL_NS.Entity.MailSettings", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext");

                    b.Property<bool>("EnableSsl")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsBodyHtml")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("NotificationId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId")
                        .IsUnique();

                    b.ToTable("MailSettings");
                });

            modelBuilder.Entity("DAL_NS.Entity.Notification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("DateTimeCreate")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DateTimeOfTheLastAttemptToSend")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Header")
                        .HasColumnType("longtext");

                    b.Property<bool>("IsSend")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("MessageBody")
                        .HasColumnType("longtext");

                    b.Property<int>("NumberOfAttemptToSent")
                        .HasColumnType("int");

                    b.Property<string>("Recipient")
                        .HasColumnType("longtext");

                    b.Property<string>("Sender")
                        .HasColumnType("longtext");

                    b.Property<int>("TypeNotification")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("DAL_NS.Entity.NotificationException", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<long?>("HResult")
                        .HasColumnType("bigint");

                    b.Property<string>("Message")
                        .HasColumnType("longtext");

                    b.Property<string>("NotificationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Type")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("NotificationId")
                        .IsUnique();

                    b.ToTable("NotificationException");
                });

            modelBuilder.Entity("DAL_NS.Entity.Credentials", b =>
                {
                    b.HasOne("DAL_NS.Entity.Notification", null)
                        .WithOne("Credentials")
                        .HasForeignKey("DAL_NS.Entity.Credentials", "NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL_NS.Entity.MailSettings", b =>
                {
                    b.HasOne("DAL_NS.Entity.Notification", null)
                        .WithOne("MailSettings")
                        .HasForeignKey("DAL_NS.Entity.MailSettings", "NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL_NS.Entity.NotificationException", b =>
                {
                    b.HasOne("DAL_NS.Entity.Notification", null)
                        .WithOne("Exception")
                        .HasForeignKey("DAL_NS.Entity.NotificationException", "NotificationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DAL_NS.Entity.Notification", b =>
                {
                    b.Navigation("Credentials");

                    b.Navigation("Exception");

                    b.Navigation("MailSettings");
                });
#pragma warning restore 612, 618
        }
    }
}
