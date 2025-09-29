using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Pages.Kanban;

public class AddItemModel : PageModel
{
    private readonly ILogger<AddItemModel> _logger;
    private readonly IKanbanDataService _kanbanDataService;

    public AddItemModel(ILogger<AddItemModel> logger, IKanbanDataService kanbanDataService)
    {
        _logger = logger;
        _kanbanDataService = kanbanDataService;
    }

    [BindProperty]
    public CreateKanbanItemForm NewItem { get; set; } = new();

    public IReadOnlyList<TeamMember> TeamMembers => _kanbanDataService.GetAllMembers();
    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Add Item page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }
    }

    public IActionResult OnPost()
    {
        _logger.LogInformation("AddItem OnPost called!");
        
        // Check if this is an AJAX request
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || 
                     Request.ContentType?.Contains("application/json") == true ||
                     Request.Headers.Accept.Any(h => h.Contains("application/json"));

        // Clear ModelState and validate only NewItem
        ModelState.Clear();
        var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(NewItem);
        var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
        var isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(NewItem, validationContext, validationResults, true);
        
        if (!isValid)
        {
            _logger.LogWarning("NewItem model is invalid");
            
            if (isAjax)
            {
                var errors = new Dictionary<string, string[]>();
                foreach (var result in validationResults)
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        errors[$"NewItem.{memberName}"] = new[] { result.ErrorMessage ?? "Invalid value" };
                    }
                }
                return new JsonResult(new { success = false, errors });
            }
            
            foreach (var result in validationResults)
            {
                foreach (var memberName in result.MemberNames)
                {
                    ModelState.AddModelError($"NewItem.{memberName}", result.ErrorMessage ?? "Invalid value");
                }
            }
            return Page();
        }

        var item = _kanbanDataService.AddItem(NewItem.Title, NewItem.Description, NewItem.AssignedToId);

        _logger.LogInformation("New Kanban item created: {Title} at {Time}", 
            item.Title, DateTime.UtcNow);

        if (isAjax)
        {
            return new JsonResult(new { 
                success = true, 
                message = "Kanban item created successfully!", 
                item = new { 
                    id = item.Id, 
                    title = item.Title, 
                    description = item.Description 
                } 
            });
        }

        TempData["SuccessMessage"] = "Kanban item created successfully!";
        return RedirectToPage();
    }
}