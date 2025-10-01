using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages.Calendar;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private static readonly List<CalendarEvent> _events = new();
    private static int _nextId = 1;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<CalendarEvent> Events => _events.AsReadOnly();

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Calendar page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }
    }

    public IActionResult OnPostDelete(int id)
    {
        var eventItem = _events.FirstOrDefault(e => e.Id == id);
        if (eventItem != null)
        {
            _events.Remove(eventItem);
            
            _logger.LogInformation("Event {Id} deleted at {Time}", id, DateTime.UtcNow);
            
            TempData["SuccessMessage"] = "Event deleted successfully!";
        }

        return RedirectToPage();
    }

    // Static methods to access events from other pages
    public static void AddEvent(CalendarEvent calendarEvent)
    {
        calendarEvent.Id = _nextId++;
        _events.Add(calendarEvent);
    }

    public static IReadOnlyList<CalendarEvent> GetAllEvents() => _events.AsReadOnly();
}

public class CalendarEvent
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public bool IsToday => StartDateTime.Date == DateTime.Today;
    public bool IsUpcoming => StartDateTime.Date > DateTime.Today;
    public bool IsPast => StartDateTime.Date < DateTime.Today;
}

public class CreateEventForm
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    [Display(Name = "Event Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Start date is required.")]
    [Display(Name = "Start Date")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "Start time is required.")]
    [Display(Name = "Start Time")]
    public TimeOnly StartTime { get; set; } = new TimeOnly(9, 0);

    [Required(ErrorMessage = "End date is required.")]
    [Display(Name = "End Date")]
    public DateOnly EndDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Required(ErrorMessage = "End time is required.")]
    [Display(Name = "End Time")]
    public TimeOnly EndTime { get; set; } = new TimeOnly(10, 0);
}
