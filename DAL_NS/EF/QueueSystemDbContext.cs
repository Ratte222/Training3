using DAL_NS.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL_NS.EF
{
    //https://docs.microsoft.com/en-us/ef/core/modeling/generated-properties?tabs=fluent-api
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
                m.Property(p => p.Id).ValueGeneratedOnAdd();
                m.Property(p => p.DateTimeCreate).ValueGeneratedOnAdd();
                m.Property(p => p.DateTimeOfTheLastAttemptToSend)
                .HasDefaultValueSql(null);
                m.HasOne(n => n.Credentials)
                //.WithOne(c => c.Notification)
                .WithOne()
                .HasForeignKey<Credentials>(c => c.NotificationId);
                m.HasOne(n => n.Exception)
                //.WithOne(e => e.Notification)
                .WithOne()
                .HasForeignKey<NotificationException>(c => c.NotificationId);
            });
            modelBuilder.Entity<Credentials>(m =>
            { 
                m.HasKey(i => i.Id);
                m.Property(p => p.Id).ValueGeneratedOnAdd();
            });
            modelBuilder.Entity<NotificationException>(m =>
            {
                m.HasKey(i => i.Id);
                m.Property(p => p.Id).ValueGeneratedOnAdd();
            });
        }
    }
}
