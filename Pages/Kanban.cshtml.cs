using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages;

public class KanbanModel : PageModel
{
    private readonly ILogger<KanbanModel> _logger;
    private readonly IKanbanDataService _kanbanDataService;

    public KanbanModel(ILogger<KanbanModel> logger, IKanbanDataService kanbanDataService)
    {
        _logger = logger;
        _kanbanDataService = kanbanDataService;
    }

    [BindProperty]
    public int EditItemId { get; set; }

    [BindProperty]
    public KanbanStatus NewStatus { get; set; }

    public IReadOnlyList<KanbanItem> KanbanItems => _kanbanDataService.GetAllItems();
    public IReadOnlyList<TeamMember> TeamMembers => _kanbanDataService.GetAllMembers();

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Kanban page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }
    }

    public IActionResult OnPostUpdateStatus()
    {
        var success = _kanbanDataService.UpdateItemStatus(EditItemId, NewStatus);
        if (success)
        {
            TempData["SuccessMessage"] = $"Item status updated to {NewStatus}!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostAssignMember(int itemId, int? memberId)
    {
        var success = _kanbanDataService.AssignItem(itemId, memberId);
        if (success)
        {
            var memberName = memberId.HasValue 
                ? _kanbanDataService.GetMemberById(memberId.Value)?.Name ?? "Unknown"
                : "Unassigned";
            
            TempData["SuccessMessage"] = $"Item assigned to {memberName}!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var success = _kanbanDataService.DeleteItem(id);
        if (success)
        {
            TempData["SuccessMessage"] = "Kanban item deleted successfully!";
        }

        return RedirectToPage();
    }

    public IEnumerable<KanbanItem> GetItemsByStatus(KanbanStatus status)
    {
        return _kanbanDataService.GetItemsByStatus(status);
    }

    public TeamMember? GetMemberById(int? memberId)
    {
        return _kanbanDataService.GetMemberById(memberId);
    }
}