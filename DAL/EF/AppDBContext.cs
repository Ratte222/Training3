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
        }
    }
}
