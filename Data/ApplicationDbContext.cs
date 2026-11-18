using Microsoft.EntityFrameworkCore;
using UavApi.Models;

namespace UavApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Drone> Drones { get; set; }
    public DbSet<FlightRecord> FlightRecords { get; set; }
    public DbSet<FlightDocument> FlightDocuments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FlightRecord>()
            .HasOne(f => f.Drone)
            .WithMany(d => d.FlightRecords)
            .HasForeignKey(f => f.DroneId);

        modelBuilder.Entity<FlightRecord>()
            .HasOne(f => f.User)
            .WithMany()
            .HasForeignKey(f => f.UserId);
    }
}