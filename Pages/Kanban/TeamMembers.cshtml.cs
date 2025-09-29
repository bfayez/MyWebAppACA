using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Kanban;

public class TeamMembersModel : PageModel
{
    private readonly ILogger<TeamMembersModel> _logger;
    private readonly IKanbanDataService _kanbanDataService;

    public TeamMembersModel(ILogger<TeamMembersModel> logger, IKanbanDataService kanbanDataService)
    {
        _logger = logger;
        _kanbanDataService = kanbanDataService;
    }

    [BindProperty]
    public CreateTeamMemberForm NewMember { get; set; } = new();

    public IReadOnlyList<TeamMember> TeamMembers => _kanbanDataService.GetAllMembers();
    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Team Members page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("AddMember OnPost called!");
        
        // Check if this is an AJAX request
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                     Request.ContentType?.Contains("application/json") == true ||
                     Request.Headers.Accept.Any(h => h.Contains("application/json"));

        // Clear ModelState and validate only NewMember
        ModelState.Clear();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(NewMember);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(NewMember, validationContext, validationResults, true);
        
        if (!isValid)
        {
            _logger.LogWarning("NewMember model is invalid");
            
            if (isAjax)
            {
                var errors = new Dictionary<string, string[]>();
                foreach (var result in validationResults)
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        errors[$"NewMember.{memberName}"] = new[] { result.ErrorMessage ?? "Invalid value" };
                    }
                }
                return new JsonResult(new { success = false, errors });
            }
            
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    ModelState.AddModelError($"NewMember.{memberName}", result.ErrorMessage ?? "Invalid value");
                }
            }
            return Page();
        }

        var member = _kanbanDataService.AddMember(NewMember.Name, NewMember.Email);

        _logger.LogInformation("New team member added: {Name} at {Time}", 
            member.Name, DateTime.UtcNow);

        if (isAjax)
        {
            return new JsonResult(new { 
                success = true, 
                message = "Team member added successfully!", 
                member = new { 
                    id = member.Id, 
                    name = member.Name, 
                    email = member.Email 
                } 
            });
        }

        TempData["SuccessMessage"] = "Team member added successfully!";
        return RedirectToPage();
    }

    public IActionResult OnPostRemoveMember(int id)
    {
        _logger.LogInformation("RemoveMember called for ID: {Id}", id);

        var success = _kanbanDataService.RemoveMember(id);
        
        if (success)
        {
            _logger.LogInformation("Team member {Id} removed successfully", id);
            
            // Check if this is an AJAX request
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                         Request.ContentType?.Contains("application/json") == true ||
                         Request.Headers.Accept.Any(h => h.Contains("application/json"));

            if (isAjax)
            {
                return new JsonResult(new { 
                    success = true, 
                    message = "Team member removed successfully!" 
                });
            }

            TempData["SuccessMessage"] = "Team member removed successfully!";
        }
        else
        {
            _logger.LogWarning("Failed to remove team member {Id}", id);
            TempData["ErrorMessage"] = "Failed to remove team member.";
        }

        return RedirectToPage();
    }

    public int GetAssignedMembersCount()
    {
        var allItems = _kanbanDataService.GetAllItems();
        return TeamMembers.Count(member => allItems.Any(item => item.AssignedToId == member.Id));
    }
}