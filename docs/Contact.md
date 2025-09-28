# Contact Page Documentation

## Overview

The Contact page provides a user-friendly form for collecting contact information from visitors. It demonstrates form handling, validation, and user experience best practices in ASP.NET Core Razor Pages.

## File Structure

### Contact.cshtml (View)
The Razor view contains the HTML markup and form structure.

### Contact.cshtml.cs (Page Model)
Contains the server-side logic, validation, and form processing.

## Features

### 🔍 Form Fields

| Field | Type | Required | Validation | Max Length |
|-------|------|----------|------------|------------|
| **Full Name** | Text | ✅ Yes | String length | 100 chars |
| **Email Address** | Email | ✅ Yes | Email format | N/A |
| **Phone Number** | Tel | ❌ Optional | Phone format | N/A |
| **Message** | Textarea | ❌ Optional | String length | 500 chars |

### 🎨 User Interface

- **Responsive Design**: Bootstrap-based layout adapting to different screen sizes
- **Form Grid**: Two-column layout for efficient space usage
- **Validation Feedback**: Real-time client-side and server-side validation
- **Success Messages**: User feedback after successful submission
- **Action Buttons**: Submit and Cancel options

### 🔒 Validation System

#### Server-Side Validation
```csharp
[Required(ErrorMessage = "Name is required.")]
[StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
[Display(Name = "Full Name")]
public string Name { get; set; } = string.Empty;

[Required(ErrorMessage = "Email address is required.")]
[EmailAddress(ErrorMessage = "Please enter a valid email address.")]
[Display(Name = "Email Address")]
public string Email { get; set; } = string.Empty;
```

#### Client-Side Validation
Automatic validation using ASP.NET Core's built-in jQuery validation integration.

## Technical Implementation

### 🏗️ Architecture Pattern

**Page Model Pattern**: Each page combines view and logic in a cohesive unit.

```csharp
public class ContactModel : PageModel
{
    [BindProperty]
    public ContactForm UserInfo { get; set; } = new();
    
    public string Message { get; set; } = string.Empty;
}
```

### 📨 Form Processing Flow

1. **GET Request**: Display empty form
2. **POST Request**: Process form submission
3. **Validation**: Check data validity
4. **Success**: Redirect with success message (PRG pattern)
5. **Error**: Redisplay form with validation errors

```csharp
public IActionResult OnPost()
{
    if (!ModelState.IsValid)
    {
        return Page(); // Redisplay form with errors
    }

    // Process form data
    _logger.LogInformation("User information submitted: {Name}, {Email}, {Phone}", 
        UserInfo.Name, UserInfo.Email, UserInfo.Phone);

    // POST-REDIRECT-GET pattern
    TempData["SuccessMessage"] = "Thank you for your submission!";
    return RedirectToPage();
}
```

### 🔄 POST-REDIRECT-GET Pattern

Prevents duplicate form submissions and provides better user experience:

1. **POST**: Form submission
2. **REDIRECT**: Server redirect to GET
3. **GET**: Display confirmation

## Data Model

### ContactForm Class
```csharp
public class ContactForm
{
    [Required, StringLength(100)]
    [Display(Name = "Full Name")]
    public string Name { get; set; } = string.Empty;

    [Required, EmailAddress]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone]
    [Display(Name = "Phone Number")]
    public string? Phone { get; set; }

    [StringLength(500)]
    [Display(Name = "Message")]
    public string? Message { get; set; }
}
```

## User Experience Features

### ✅ Success Handling
- Success message displayed after form submission
- Form clears after successful submission
- Logging of submissions for analytics

### ❌ Error Handling
- Individual field validation messages
- Preserves user input on validation errors
- Clear, user-friendly error messages

### 📱 Responsive Design
- Mobile-first approach
- Form adapts to screen size
- Touch-friendly interface

## Security Considerations

### ✅ Implemented Security Features

1. **Input Validation**: All inputs validated server-side
2. **Anti-Forgery Tokens**: Automatic CSRF protection
3. **Data Sanitization**: Framework handles HTML encoding
4. **Length Limits**: Prevents oversized submissions

### 🔒 Security Best Practices

- All user input is validated
- No direct database interaction (logging only)
- Framework-provided CSRF protection
- Input length limitations

## Logging and Monitoring

### 📊 Logging Implementation
```csharp
_logger.LogInformation("Contact page visited at {Time}", DateTime.UtcNow);
_logger.LogInformation("User information submitted: {Name}, {Email}, {Phone}", 
    UserInfo.Name, UserInfo.Email, UserInfo.Phone);
```

### 📈 Metrics Tracked
- Page visits
- Form submissions
- Validation failures (via framework)

## Customization Options

### 🎨 Styling Customization
- Bootstrap classes can be modified
- Custom CSS can be added to `site.css`
- Form layout can be adjusted

### 🔧 Functional Extensions
- Email sending capability
- Database storage
- File upload support
- Additional form fields

### 📧 Integration Opportunities
- Email service integration
- CRM system connection
- Database persistence
- Third-party form services

## Testing Considerations

### 🧪 Unit Testing
```csharp
[Test]
public void OnPost_ValidData_ReturnsRedirect()
{
    // Arrange
    var model = new ContactModel(logger);
    model.UserInfo = new ContactForm 
    { 
        Name = "Test User", 
        Email = "test@example.com" 
    };

    // Act
    var result = model.OnPost();

    // Assert
    Assert.IsInstanceOf<RedirectToPageResult>(result);
}
```

### 🔍 Integration Testing
- Form submission testing
- Validation behavior testing
- Success message display testing

## Accessibility Features

### ♿ WCAG Compliance
- Semantic HTML structure
- Form labels properly associated
- Color contrast compliance
- Keyboard navigation support

### 🎯 Accessibility Elements
- `role="alert"` for success messages
- Proper label associations with `asp-for`
- Logical tab order
- Screen reader friendly validation messages

## Performance Considerations

### ⚡ Optimization Features
- Minimal server processing
- Efficient validation
- Client-side validation reduces server load
- Lightweight form structure

### 📊 Performance Metrics
- Fast form rendering
- Quick validation responses
- Minimal resource usage

## Related Documentation

- [Architecture](architecture.md) - Application structure
- [Development](development.md) - Development guidelines
- [Layout](Layout.md) - Shared layout documentation
- [SiteCSS](SiteCSS.md) - Styling guidelines

---

This contact form provides a solid foundation for user interaction while maintaining security, accessibility, and performance best practices.