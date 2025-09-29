using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages;

public class KanbanModel : PageModel
{
    private readonly ILogger<KanbanModel> _logger;
    private static readonly List<KanbanItem> _kanbanItems = new();
    private static readonly List<TeamMember> _teamMembers = new();
    private static int _nextItemId = 1;
    private static int _nextMemberId = 1;

    public KanbanModel(ILogger<KanbanModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public CreateKanbanItemForm NewItem { get; set; } = new();

    [BindProperty]
    public CreateTeamMemberForm NewMember { get; set; } = new();

    [BindProperty]
    public int EditItemId { get; set; }

    [BindProperty]
    public KanbanStatus NewStatus { get; set; }

    public IReadOnlyList<KanbanItem> KanbanItems => _kanbanItems.AsReadOnly();
    public IReadOnlyList<TeamMember> TeamMembers => _teamMembers.AsReadOnly();

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

    [BindProperty]
    public string? SubmitAction { get; set; }

    public IActionResult OnPost()
    {
        _logger.LogInformation("OnPost called! SubmitAction: {Action}, ModelState.IsValid: {IsValid}", SubmitAction, ModelState.IsValid);
        
        if (SubmitAction == "AddItem")
        {
            return HandleAddItem();
        }
        else if (SubmitAction == "AddMember")
        {
            return HandleAddMember();
        }
        
        _logger.LogWarning("Unknown submit action: {Action}", SubmitAction);
        return Page();
    }

    private IActionResult HandleAddItem()
    {
        _logger.LogInformation("HandleAddItem called!");
        
        // Clear ModelState and validate only NewItem
        ModelState.Clear();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(NewItem);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(NewItem, validationContext, validationResults, true);
        
        if (!isValid)
        {
            _logger.LogWarning("NewItem model is invalid");
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    ModelState.AddModelError($"NewItem.{memberName}", result.ErrorMessage ?? "Invalid value");
                }
            }
            return Page();
        }

        var item = new KanbanItem
        {
            Id = _nextItemId++,
            Title = NewItem.Title,
            Description = NewItem.Description,
            Status = KanbanStatus.New,
            AssignedToId = NewItem.AssignedToId,
            CreatedAt = DateTime.UtcNow
        };

        _kanbanItems.Add(item);

        _logger.LogInformation("New Kanban item created: {Title} at {Time}", 
            item.Title, DateTime.UtcNow);

        TempData["SuccessMessage"] = "Kanban item created successfully!";
        
        return RedirectToPage();
    }

    private IActionResult HandleAddMember()
    {
        _logger.LogInformation("HandleAddMember called!");
        
        // Clear ModelState and validate only NewMember
        ModelState.Clear();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(NewMember);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(NewMember, validationContext, validationResults, true);
        
        if (!isValid)
        {
            _logger.LogWarning("NewMember model is invalid");
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    ModelState.AddModelError($"NewMember.{memberName}", result.ErrorMessage ?? "Invalid value");
                }
            }
            return Page();
        }

        var member = new TeamMember
        {
            Id = _nextMemberId++,
            Name = NewMember.Name,
            Email = NewMember.Email,
            CreatedAt = DateTime.UtcNow
        };

        _teamMembers.Add(member);

        _logger.LogInformation("New team member added: {Name} at {Time}", 
            member.Name, DateTime.UtcNow);

        TempData["SuccessMessage"] = "Team member added successfully!";
        
        return RedirectToPage();
    }

    public IActionResult OnPostUpdateStatus()
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == EditItemId);
        if (item != null)
        {
            var oldStatus = item.Status;
            item.Status = NewStatus;
            item.UpdatedAt = DateTime.UtcNow;
            
            _logger.LogInformation("Kanban item {Id} status changed from {OldStatus} to {NewStatus} at {Time}", 
                EditItemId, oldStatus, NewStatus, DateTime.UtcNow);

            TempData["SuccessMessage"] = $"Item status updated to {NewStatus}!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostAssignMember(int itemId, int? memberId)
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            item.AssignedToId = memberId;
            item.UpdatedAt = DateTime.UtcNow;
            
            var memberName = memberId.HasValue 
                ? _teamMembers.FirstOrDefault(m => m.Id == memberId.Value)?.Name ?? "Unknown"
                : "Unassigned";
            
            _logger.LogInformation("Kanban item {Id} assigned to {Member} at {Time}", 
                itemId, memberName, DateTime.UtcNow);

            TempData["SuccessMessage"] = $"Item assigned to {memberName}!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var item = _kanbanItems.FirstOrDefault(i => i.Id == id);
        if (item != null)
        {
            _kanbanItems.Remove(item);
            
            _logger.LogInformation("Kanban item {Id} deleted at {Time}", id, DateTime.UtcNow);
            
            TempData["SuccessMessage"] = "Kanban item deleted successfully!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostRemoveMember(int id)
    {
        var member = _teamMembers.FirstOrDefault(m => m.Id == id);
        if (member != null)
        {
            // Unassign this member from all items
            foreach (var item in _kanbanItems.Where(i => i.AssignedToId == id))
            {
                item.AssignedToId = null;
            }

            _teamMembers.Remove(member);
            
            _logger.LogInformation("Team member {Id} removed at {Time}", id, DateTime.UtcNow);
            
            TempData["SuccessMessage"] = "Team member removed successfully!";
        }

        return RedirectToPage();
    }

    public IEnumerable<KanbanItem> GetItemsByStatus(KanbanStatus status)
    {
        return _kanbanItems.Where(i => i.Status == status).OrderByDescending(i => i.CreatedAt);
    }

    public TeamMember? GetMemberById(int? memberId)
    {
        return memberId.HasValue ? _teamMembers.FirstOrDefault(m => m.Id == memberId.Value) : null;
    }
}

public enum KanbanStatus
{
    New,
    Active,
    Blocked,
    Completed
}

public class KanbanItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public KanbanStatus Status { get; set; }
    public int? AssignedToId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TeamMember
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateKanbanItemForm
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    [Display(Name = "Item Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }

    [Display(Name = "Assign To")]
    public int? AssignedToId { get; set; }
}

public class CreateTeamMemberForm
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    [Display(Name = "Member Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}