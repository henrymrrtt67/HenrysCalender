using System.ComponentModel.DataAnnotations;
using HenrysCalendar.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HenrysCalendar.Pages;

public class IndexModel(BookingService svc) : PageModel
{
    public class BookingInput
    {
        [Required] public string Name { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public DateOnly Date { get; set; }
        [Required] public TimeOnly StartTime { get; set; }
        [Required, Range(1, 1440)] public int Minutes { get; set; } = 60;
        public string? Notes { get; set; }
    }

    [BindProperty] public BookingInput Input { get; set; } = new();
    [TempData] public string? Message { get; set; }
    public List<(DateTime Start, DateTime End)> Upcoming { get; set; } = new();

    public async Task OnGetAsync()
    {
        Input.Date = DateOnly.FromDateTime(BookingService.ToMelbourne(DateTime.UtcNow));
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            var start = Input.Date.ToDateTime(Input.StartTime);
            var error = await svc.TryCreateAsync(Input.Name, Input.Email, Input.Notes, start, Input.Minutes);
            if (error is null)
            {
                Message = "Booked! See you then.";
                return RedirectToPage();
            }
            ModelState.AddModelError(string.Empty, error);
        }
        await LoadAsync();
        return Page();
    }

    private async Task LoadAsync() =>
        Upcoming = (await svc.GetUpcomingAsync())
            .Select(b => (BookingService.ToMelbourne(b.StartUtc), BookingService.ToMelbourne(b.EndUtc))).ToList();
}
