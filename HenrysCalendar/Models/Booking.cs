using System.ComponentModel.DataAnnotations;

namespace HenrysCalendar.Models;

public class Booking
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = "";

    [Required, EmailAddress, StringLength(200)]
    public string Email { get; set; } = "";

    [StringLength(1000)]
    public string? Notes { get; set; }

    // Always stored in UTC. Converted to Melbourne time only for display/input.
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public bool Cancelled { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
