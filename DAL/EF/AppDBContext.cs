using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.EF
{
    public class AppDBContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Pupil> Pupils { get; set; }
        public DbSet<AcademicSubject> AcademicSubjects { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<PupilAcademicSubject> PupilAcademicSubjects { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Expense>(m =>
            {
                m.HasKey(p => p.Id);
                m.Property(p => p.Id).HasColumnName("id");
                m.HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryId);
                
            });
            modelBuilder.Entity<Category>(m =>
            {
                m.HasKey(p => p.Id);
                m.Property(p => p.Id).HasColumnName("id");
            });
            modelBuilder.Entity<Pupil>(p =>
            {
                p.HasKey(k => k.Id);
                p.HasOne(p => p.SchoolClass)
                .WithMany(sc => sc.Pupils)
                .HasForeignKey(fk => fk.SchoolClassId);
                p.HasMany(pa => pa.PupilAcademicSubjects)
                .WithOne(p => p.Pupil)
                .HasForeignKey(fk => fk.PupilId);
            });
            modelBuilder.Entity<PupilAcademicSubject>(m =>
            {
                m.HasKey(i => new { i.PupilId, i.AcademicSubjectId });
            });
            modelBuilder.Entity<AcademicSubject>(m =>
            {
                m.HasKey(i => i.Id);
                m.HasMany(j => j.PupilAcademicSubjects)
                .WithOne(n => n.AcademicSubject)
                .HasForeignKey(fk => fk.AcademicSubjectId);
            });
            modelBuilder.Entity<SchoolClass>(m =>
            {
                m.HasKey(i => i.Id);
            });
        }
    }
}
