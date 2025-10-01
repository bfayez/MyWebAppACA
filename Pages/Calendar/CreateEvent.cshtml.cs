using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWebApp.Pages.Calendar;

public class CreateEventModel : PageModel
{
    private readonly ILogger<CreateEventModel> _logger;

    public CreateEventModel(ILogger<CreateEventModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public CreateEventForm NewEvent { get; set; } = new();

    public string Message { get; set; } = string.Empty;
    
    public int EventsAddedThisSession { get; set; } = 0;

    public void OnGet()
    {
        _logger.LogInformation("Create Event page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }

        // Track events added this session
        if (TempData["EventsAddedThisSession"] is int count)
        {
            EventsAddedThisSession = count;
            TempData.Keep("EventsAddedThisSession"); // Keep for the next request
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            // Preserve the count on validation errors
            if (TempData["EventsAddedThisSession"] is int count)
            {
                EventsAddedThisSession = count;
                TempData.Keep("EventsAddedThisSession");
            }
            return Page();
        }

        var calendarEvent = new CalendarEvent
        {
            Title = NewEvent.Title,
            Description = NewEvent.Description,
            StartDateTime = NewEvent.StartDate.ToDateTime(NewEvent.StartTime),
            EndDateTime = NewEvent.EndDate.ToDateTime(NewEvent.EndTime),
            CreatedAt = DateTime.UtcNow
        };

        IndexModel.AddEvent(calendarEvent);

        _logger.LogInformation("New event created: {Title} at {Time}", 
            calendarEvent.Title, DateTime.UtcNow);

        // Track events added this session
        int eventsAdded = TempData["EventsAddedThisSession"] is int sessionCount ? sessionCount : 0;
        eventsAdded++;
        TempData["EventsAddedThisSession"] = eventsAdded;

        TempData["SuccessMessage"] = $"Event '{calendarEvent.Title}' created successfully!";
        
        // Redirect to GET to clear the form (POST-REDIRECT-GET pattern)
        return RedirectToPage();
    }
}
