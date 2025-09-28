# Development Guide

This guide provides comprehensive information for developers working on the MyWebApp project.

## 🛠️ Development Environment Setup

### Prerequisites

Ensure you have the following tools installed:

| Tool | Version | Purpose |
|------|---------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ | Core development framework |
| [Visual Studio 2022](https://visualstudio.microsoft.com/) | 17.8+ | Primary IDE (recommended) |
| [Visual Studio Code](https://code.visualstudio.com/) | Latest | Alternative lightweight editor |
| [Git](https://git-scm.com/) | Latest | Version control |
| [PowerShell](https://github.com/PowerShell/PowerShell) | 7.0+ | Cross-platform shell |

### Optional Tools

| Tool | Purpose |
|------|---------|
| [Docker Desktop](https://www.docker.com/products/docker-desktop) | Container development and testing |
| [Azure CLI](https://docs.microsoft.com/cli/azure/) | Azure resource management |
| [Postman](https://www.postman.com/) | API testing (for future API development) |
| [SQL Server Management Studio](https://docs.microsoft.com/sql/ssms/) | Database management (if database added) |

### IDE Configuration

#### Visual Studio 2022 Setup

1. **Required Workloads:**
   - ASP.NET and web development
   - Azure development (for deployment)
   - .NET desktop development

2. **Recommended Extensions:**
   - Web Essentials
   - CodeMaid
   - Productivity Power Tools
   - Azure DevOps Extension

#### Visual Studio Code Setup

1. **Required Extensions:**
   ```json
   {
     "recommendations": [
       "ms-dotnettools.csharp",
       "ms-dotnettools.csdevkit",
       "ms-vscode.vscode-json",
       "ms-azuretools.vscode-azureappservice",
       "bradlc.vscode-tailwindcss"
     ]
   }
   ```

2. **Workspace Settings (`.vscode/settings.json`):**
   ```json
   {
     "dotnet.defaultSolution": "MyWebApp.sln",
     "omnisharp.enableRoslynAnalyzers": true,
     "files.exclude": {
       "**/bin": true,
       "**/obj": true
     }
   }
   ```

## 🚀 Getting Started

### Initial Setup

1. **Clone the Repository:**
   ```bash
   git clone <repository-url>
   cd MyWebAppACA
   ```

2. **Restore Dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the Solution:**
   ```bash
   dotnet build
   ```

4. **Run the Application:**
   ```bash
   dotnet run
   ```

   The application will be available at:
   - HTTP: `http://localhost:5000`
   - HTTPS: `https://localhost:5001`

### Development Commands

| Command | Description |
|---------|-------------|
| `dotnet run` | Run the application |
| `dotnet watch` | Run with hot reload |
| `dotnet build` | Build the solution |
| `dotnet test` | Run unit tests |
| `dotnet clean` | Clean build artifacts |
| `dotnet restore` | Restore NuGet packages |

## 🔥 Hot Reload and Live Development

### .NET Hot Reload

Hot Reload allows you to modify source files and see changes immediately without restarting the application.

**Enable Hot Reload:**
```bash
dotnet watch run
```

**Supported Changes:**
- Method implementations
- Property changes
- Adding new methods
- CSS and JavaScript modifications
- Razor view updates

**Unsupported Changes:**
- Assembly references
- New dependencies
- Startup configuration changes

### Browser Refresh

The application is configured to automatically refresh the browser when files change:

```csharp
// In Program.cs (Development environment)
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    // Browser refresh is automatically enabled
}
```

## 🐛 Debugging

### Visual Studio Debugging

1. **Set Breakpoints:** Click in the left margin or press `F9`
2. **Start Debugging:** Press `F5` or click "Start"
3. **Debug Navigation:**
   - `F10`: Step Over
   - `F11`: Step Into
   - `Shift+F11`: Step Out
   - `F5`: Continue

### Visual Studio Code Debugging

1. **Launch Configuration (`.vscode/launch.json`):**
   ```json
   {
     "version": "0.2.0",
     "configurations": [
       {
         "name": ".NET Core Launch (web)",
         "type": "coreclr",
         "request": "launch",
         "preLaunchTask": "build",
         "program": "${workspaceFolder}/bin/Debug/net8.0/MyWebApp.dll",
         "args": [],
         "cwd": "${workspaceFolder}",
         "stopAtEntry": false,
         "serverReadyAction": {
           "action": "openExternally",
           "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
         },
         "env": {
           "ASPNETCORE_ENVIRONMENT": "Development"
         }
       }
     ]
   }
   ```

### Debugging Techniques

#### Page Model Debugging

```csharp
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    
    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }
    
    public void OnGet()
    {
        // Set breakpoint here
        _logger.LogInformation("Index page accessed at {Time}", DateTime.Now);
        
        // Debug variables in locals window
        var userAgent = Request.Headers["User-Agent"].ToString();
        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
    }
}
```

#### Client-Side Debugging

1. **Browser Developer Tools:** `F12`
2. **Console Logging:** Add `console.log()` statements
3. **Network Tab:** Monitor HTTP requests
4. **Sources Tab:** Set JavaScript breakpoints

## 🧪 Testing

### Unit Testing Setup

Currently, the project doesn't include tests, but here's how to add them:

1. **Create Test Project:**
   ```bash
   dotnet new xunit -n MyWebApp.Tests
   dotnet add MyWebApp.Tests/MyWebApp.Tests.csproj reference MyWebApp/MyWebApp.csproj
   dotnet sln add MyWebApp.Tests/MyWebApp.Tests.csproj
   ```

2. **Add Testing Packages:**
   ```bash
   dotnet add MyWebApp.Tests package Microsoft.AspNetCore.Mvc.Testing
   dotnet add MyWebApp.Tests package FluentAssertions
   ```

### Example Unit Test

```csharp
public class IndexPageTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public IndexPageTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task Get_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Act
        var response = await client.GetAsync("/");
        
        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("text/html; charset=utf-8", 
            response.Content.Headers.ContentType?.ToString());
    }
}
```

### Integration Testing

```csharp
[Fact]
public async Task HomePage_ContainsWelcomeMessage()
{
    // Arrange
    var client = _factory.CreateClient();
    
    // Act
    var response = await client.GetAsync("/");
    var content = await response.Content.ReadAsStringAsync();
    
    // Assert
    content.Should().Contain("Welcome");
}
```

## 📝 Code Style and Standards

### Coding Conventions

Follow the [.NET Coding Conventions](https://docs.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions):

#### C# Style Rules

```csharp
// Use var when type is obvious
var items = new List<string>();

// Use explicit types when not obvious
StreamReader reader = File.OpenText("file.txt");

// Method naming: PascalCase
public void ProcessUserRequest() { }

// Property naming: PascalCase
public string UserName { get; set; }

// Field naming: camelCase with underscore prefix for private
private readonly ILogger _logger;

// Constants: PascalCase
public const string DefaultConnectionString = "...";
```

#### Razor Page Conventions

```html
<!-- Use semantic HTML -->
<main role="main">
    <section class="hero">
        <h1>Page Title</h1>
    </section>
</main>

<!-- Use Bootstrap classes consistently -->
<div class="container">
    <div class="row">
        <div class="col-md-8">
            <!-- Content -->
        </div>
    </div>
</div>
```

### EditorConfig

The project uses `.editorconfig` for consistent formatting:

```ini
root = true

[*]
charset = utf-8
insert_final_newline = true
trim_trailing_whitespace = true

[*.cs]
indent_style = space
indent_size = 4

[*.{js,css,html,cshtml}]
indent_style = space
indent_size = 2
```

## 🔧 Configuration Management

### Development Configuration

#### appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Routing": "Information"
    }
  },
  "DetailedErrors": true,
  "AllowedHosts": "*"
}
```

#### User Secrets

For sensitive development data:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
```

### Environment Variables

Common development environment variables:

```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Development

# Enable detailed errors
export ASPNETCORE_DETAILEDERRORS=true

# Set log level
export Logging__LogLevel__Default=Debug
```

## 📦 Package Management

### Adding New Packages

```bash
# Add a new package
dotnet add package PackageName

# Add specific version
dotnet add package PackageName -v 1.2.3

# Add development-only package
dotnet add package PackageName --no-restore
```

### Updating Packages

```bash
# List outdated packages
dotnet list package --outdated

# Update all packages
dotnet add package PackageName

# Update to specific version
dotnet add package PackageName -v 2.0.0
```

## 🎨 Frontend Development

### CSS Development

The project uses custom CSS with Bootstrap framework:

#### File Structure
```
wwwroot/css/
├── site.css          # Custom application styles
└── site.css.map      # Source map (if using Sass)
```

#### CSS Organization
```css
/* Variables */
:root {
  --primary-color: #007bff;
  --secondary-color: #6c757d;
}

/* Base styles */
body {
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

/* Component styles */
.custom-header {
  background-color: var(--primary-color);
}

/* Utility classes */
.text-primary-custom {
  color: var(--primary-color) !important;
}
```

### JavaScript Development

#### File Structure
```
wwwroot/js/
├── site.js           # Custom application scripts
├── modules/          # ES6 modules (if used)
└── vendor/           # Third-party scripts
```

#### JavaScript Patterns
```javascript
// Use strict mode
'use strict';

// Namespace your code
var MyWebApp = MyWebApp || {};

MyWebApp.Utils = {
    // Utility functions
    formatDate: function(date) {
        return date.toLocaleDateString();
    }
};

// Initialize on DOM ready
document.addEventListener('DOMContentLoaded', function() {
    MyWebApp.init();
});
```

## 🔍 Performance Optimization

### Development Performance

#### Hot Reload Optimization

```json
// In launchSettings.json
{
  "profiles": {
    "MyWebApp": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "hotReloadProfile": "aspnetcore",
      "applicationUrl": "https://localhost:5001;http://localhost:5000"
    }
  }
}
```

#### Build Performance

```xml
<!-- In MyWebApp.csproj for faster builds -->
<PropertyGroup Condition="'$(Configuration)'=='Debug'">
  <DebugType>portable</DebugType>
  <DebugSymbols>true</DebugSymbols>
</PropertyGroup>
```

### Runtime Performance Monitoring

```csharp
// Add performance logging
public void OnGet()
{
    var stopwatch = Stopwatch.StartNew();
    
    // Your code here
    
    stopwatch.Stop();
    _logger.LogInformation("Page load took {ElapsedMs}ms", 
        stopwatch.ElapsedMilliseconds);
}
```

## 🚨 Error Handling

### Development Error Pages

The application shows detailed error information in development:

```csharp
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
```

### Custom Error Handling

```csharp
// In Error.cshtml.cs
public class ErrorModel : PageModel
{
    public string? RequestId { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
    }
}
```

## 📋 Development Checklist

Before committing code, ensure:

- [ ] Code builds without warnings
- [ ] All tests pass (when tests exist)
- [ ] Code follows style conventions
- [ ] No sensitive information in code
- [ ] Documentation updated if needed
- [ ] Browser testing completed
- [ ] Performance impact considered

## 🔗 Useful Development Resources

### Documentation Links

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Razor Pages Documentation](https://docs.microsoft.com/aspnet/core/razor-pages)
- [Entity Framework Core](https://docs.microsoft.com/ef/core) (for future database integration)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security)

### Tools and Utilities

- [.NET CLI Reference](https://docs.microsoft.com/dotnet/core/tools/)
- [NuGet Package Manager](https://www.nuget.org/)
- [Bootstrap Documentation](https://getbootstrap.com/docs/)
- [MDN Web Docs](https://developer.mozilla.org/) (for HTML/CSS/JS)

### Community Resources

- [.NET Foundation](https://dotnetfoundation.org/)
- [ASP.NET Community](https://dotnet.microsoft.com/platform/community)
- [Stack Overflow - ASP.NET Core](https://stackoverflow.com/questions/tagged/asp.net-core)

---

Happy coding! 🚀