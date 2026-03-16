using Microsoft.EntityFrameworkCore;
using RockstarAPI.Models;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace RockstarAPI.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Trainer> Trainers { get; set; }
    public DbSet<Direction> Directions { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // Trainers
        modelBuilder.Entity<Trainer>()
            .HasIndex(t => t.Email)
            .IsUnique();

        // Schedule - связи
        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Trainer)
            .WithMany()
            .HasForeignKey(s => s.TrainerId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Direction)
            .WithMany()
            .HasForeignKey(s => s.DirectionId);

        modelBuilder.Entity<Schedule>()
            .HasOne(s => s.Service)
            .WithMany()
            .HasForeignKey(s => s.ServiceId)
            .OnDelete(DeleteBehavior.SetNull);

        // Enrollments - уникальность
        modelBuilder.Entity<Enrollment>()
            .HasIndex(e => new { e.UserId, e.ScheduleId })
            .IsUnique();

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);

        modelBuilder.Entity<Enrollment>()
            .HasOne(e => e.Schedule)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(e => e.ScheduleId);
    }
}