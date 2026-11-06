using HenrysCalendar.Models;
using Microsoft.EntityFrameworkCore;

namespace HenrysCalendar.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Booking> Bookings => Set<Booking>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Booking>().HasIndex(x => new { x.StartUtc, x.EndUtc });
    }
}
