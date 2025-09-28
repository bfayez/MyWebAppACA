using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages;

public class TodosModel : PageModel
{
    private readonly ILogger<TodosModel> _logger;
    private static readonly List<TodoItem> _todos = new();
    private static int _nextId = 1;

    public TodosModel(ILogger<TodosModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public CreateTodoForm NewTodo { get; set; } = new();

    public IReadOnlyList<TodoItem> Todos => _todos.AsReadOnly();

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Todos page visited at {Time}", DateTime.UtcNow);
        
        // Display success message from TempData if available
        if (TempData["SuccessMessage"] != null)
        {
            Message = TempData["SuccessMessage"]?.ToString() ?? string.Empty;
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var todo = new TodoItem
        {
            Id = _nextId++,
            Title = NewTodo.Title,
            Description = NewTodo.Description,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _todos.Add(todo);

        _logger.LogInformation("New todo created: {Title} at {Time}", 
            todo.Title, DateTime.UtcNow);

        TempData["SuccessMessage"] = "Todo item created successfully!";
        
        // Redirect to GET to clear the form (POST-REDIRECT-GET pattern)
        return RedirectToPage();
    }

    public IActionResult OnPostToggleComplete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo != null)
        {
            todo.IsCompleted = !todo.IsCompleted;
            todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
            
            _logger.LogInformation("Todo {Id} marked as {Status} at {Time}", 
                id, todo.IsCompleted ? "completed" : "incomplete", DateTime.UtcNow);

            TempData["SuccessMessage"] = $"Todo item marked as {(todo.IsCompleted ? "completed" : "incomplete")}!";
        }

        return RedirectToPage();
    }

    public IActionResult OnPostDelete(int id)
    {
        var todo = _todos.FirstOrDefault(t => t.Id == id);
        if (todo != null)
        {
            _todos.Remove(todo);
            
            _logger.LogInformation("Todo {Id} deleted at {Time}", id, DateTime.UtcNow);
            
            TempData["SuccessMessage"] = "Todo item deleted successfully!";
        }

        return RedirectToPage();
    }
}

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateTodoForm
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
    [Display(Name = "Todo Title")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Display(Name = "Description")]
    public string? Description { get; set; }
}