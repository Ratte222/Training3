﻿// <auto-generated />
using System;
using DAL.EF;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.19")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("DAL.Entity.AcademicSubject", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("AcademicSubjects");
                });

            modelBuilder.Entity("DAL.Entity.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<string>("Aliases")
                        .HasColumnName("aliases")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Codename")
                        .HasColumnName("codename")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.Property<bool>("Is_base_expense")
                        .HasColumnName("is_base_expense")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Is_base_income")
                        .HasColumnName("is_base_income")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("Is_income")
                        .HasColumnName("is_income")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4")
                        .HasMaxLength(255);

                    b.HasKey("Id");

                    b.ToTable("category");
                });

            modelBuilder.Entity("DAL.Entity.Expense", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnName("id")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnName("amount")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnName("category_codename")
                        .HasColumnType("int");

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Raw_text")
                        .HasColumnName("raw_text")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("expense");
                });

            modelBuilder.Entity("DAL.Entity.Pupil", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("FirstName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("LastName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("SchoolClassId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SchoolClassId");

                    b.ToTable("Pupils");
                });

            modelBuilder.Entity("DAL.Entity.PupilAcademicSubject", b =>
                {
                    b.Property<int>("PupilId")
                        .HasColumnType("int");

                    b.Property<int>("AcademicSubjectId")
                        .HasColumnType("int");

                    b.HasKey("PupilId", "AcademicSubjectId");

                    b.HasIndex("AcademicSubjectId");

                    b.ToTable("PupilAcademicSubjects");
                });

            modelBuilder.Entity("DAL.Entity.SchoolClass", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("SchoolClasses");
                });

            modelBuilder.Entity("DAL.Entity.Expense", b =>
                {
                    b.HasOne("DAL.Entity.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DAL.Entity.Pupil", b =>
                {
                    b.HasOne("DAL.Entity.SchoolClass", "SchoolClass")
                        .WithMany("Pupils")
                        .HasForeignKey("SchoolClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("DAL.Entity.PupilAcademicSubject", b =>
                {
                    b.HasOne("DAL.Entity.AcademicSubject", "AcademicSubject")
                        .WithMany("PupilAcademicSubjects")
                        .HasForeignKey("AcademicSubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entity.Pupil", "Pupil")
                        .WithMany("PupilAcademicSubjects")
                        .HasForeignKey("PupilId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
