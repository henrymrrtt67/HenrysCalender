using HenrysCalendar.Data;
using HenrysCalendar.Models;
using Microsoft.EntityFrameworkCore;

namespace HenrysCalendar.Services;

public class BookingService(AppDbContext db)
{
    // Windows uses "AUS Eastern Standard Time"; Linux/macOS use the IANA id. .NET 8 accepts both on most setups.
    public static readonly TimeZoneInfo Melbourne = FindZone();
    private static readonly SemaphoreSlim Gate = new(1, 1); // stops two simultaneous requests grabbing the same slot

    private static TimeZoneInfo FindZone()
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById("Australia/Melbourne"); }
        catch { return TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time"); }
    }

    public async Task<string?> TryCreateAsync(string name, string email, string? notes,
                                              DateTime startLocal, int minutes)
    {
        startLocal = DateTime.SpecifyKind(startLocal, DateTimeKind.Unspecified);

        if (minutes < 1) return "Duration must be at least 1 minute.";
        if (Melbourne.IsInvalidTime(startLocal)) return "That time doesn't exist (daylight saving change).";

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(startLocal, Melbourne);
        var endUtc = startUtc.AddMinutes(minutes);
        var endLocal = TimeZoneInfo.ConvertTimeFromUtc(endUtc, Melbourne);

        if (startUtc < DateTime.UtcNow) return "That time is in the past.";

        // Rule: appointment must finish by midnight (ending exactly at 12:00 AM is allowed).
        if (endLocal > startLocal.Date.AddDays(1))
            return "Appointments can't go past midnight.";

        await Gate.WaitAsync();
        try
        {
            // Rule: one booking per slot (no overlaps with any active booking).
            var clash = await db.Bookings.AnyAsync(b =>
                !b.Cancelled && b.StartUtc < endUtc && b.EndUtc > startUtc);
            if (clash) return "That time overlaps an existing booking.";

            db.Bookings.Add(new Booking
            {
                Name = name.Trim(), Email = email.Trim(), Notes = notes,
                StartUtc = startUtc, EndUtc = endUtc
            });
            await db.SaveChangesAsync();
            return null; // success
        }
        finally { Gate.Release(); }
    }

    public async Task<List<Booking>> GetUpcomingAsync() =>
        await db.Bookings.Where(b => !b.Cancelled && b.EndUtc > DateTime.UtcNow)
                         .OrderBy(b => b.StartUtc).ToListAsync();

    public static DateTime ToMelbourne(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Melbourne);
}
