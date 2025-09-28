using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Pages;

public class ContactModel : PageModel
{
    private readonly ILogger<ContactModel> _logger;

    public ContactModel(ILogger<ContactModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public ContactForm UserInfo { get; set; } = new();

    public string Message { get; set; } = string.Empty;

    public void OnGet()
    {
        _logger.LogInformation("Contact page visited at {Time}", DateTime.UtcNow);
        
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

        _logger.LogInformation("User information submitted: {Name}, {Email}, {Phone}", 
            UserInfo.Name, UserInfo.Email, UserInfo.Phone);

        // Use TempData to show success message after redirect
        TempData["SuccessMessage"] = "Thank you for your submission! We have received your information.";
        
        // Redirect to GET to clear the form (POST-REDIRECT-GET pattern)
        return RedirectToPage();
    }
}

public class ContactForm
{
    [Required(ErrorMessage = "Name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Please enter a valid phone number.")]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(500, ErrorMessage = "Message cannot exceed 500 characters.")]
    [Display(Name = "Message")]
    public string? Message { get; set; }
}