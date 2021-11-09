using DAL.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.EF
{
    public class QueueSystemDbContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        public QueueSystemDbContext(DbContextOptions<QueueSystemDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Notification>(m =>
            {
                m.HasKey(k => k.Id);
                m.Property(p => p.DateTimeOfTheLastAttemptToSend)
                .HasDefaultValueSql(null);
            });
        }
    }
}
