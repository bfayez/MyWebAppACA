# Program.cs - Application Entry Point

## Overview

`Program.cs` is the entry point for the ASP.NET Core web application. This file contains the minimal hosting model introduced in .NET 6+ and configures the entire application pipeline.

## Structure and Flow

### Application Builder Creation
```csharp
var builder = WebApplication.CreateBuilder(args);
```
Creates a `WebApplicationBuilder` with:
- Pre-configured defaults for logging, configuration, and dependency injection
- Command-line arguments processing
- Environment variable configuration

### Service Registration
```csharp
builder.Services.AddRazorPages();
```
Registers Razor Pages services in the dependency injection container:
- Page model activation
- Razor view engine
- Model binding services
- Validation services

### Application Build
```csharp
var app = builder.Build();
```
Creates the `WebApplication` instance with all configured services.

## Middleware Pipeline Configuration

The middleware pipeline processes requests in the order they are configured:

### 1. Development Exception Handling
```csharp
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
```
- **Development**: Shows detailed exception pages with stack traces
- **Production**: Redirects errors to `/Error` page and enables HSTS

### 2. HTTPS and Security
```csharp
app.UseHttpsRedirection();
```
Automatically redirects HTTP requests to HTTPS for security.

### 3. Static File Serving
```csharp
app.UseStaticFiles();
```
Serves static files from `wwwroot/` directory (CSS, JavaScript, images).

### 4. Routing
```csharp
app.UseRouting();
```
Enables request routing to match URLs to endpoints.

### 5. Authorization
```csharp
app.UseAuthorization();
```
Processes authorization policies (currently no authentication configured).

### 6. Razor Pages Mapping
```csharp
app.MapRazorPages();
```
Maps Razor Pages to their corresponding routes using conventions.

### 7. Application Start
```csharp
app.Run();
```
Starts the web server and begins processing requests.

## Key Features

### Environment-Aware Configuration
The application automatically adjusts behavior based on the hosting environment:
- **Development**: Detailed errors, hot reload, browser refresh
- **Production**: Error handling, security headers, performance optimizations

### Minimal API Surface
The file uses the minimal hosting model, reducing boilerplate code while maintaining full functionality.

### Built-in Dependency Injection
All services are registered and resolved through the built-in DI container.

## Configuration Sources

The application loads configuration from multiple sources (in priority order):
1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. Environment variables
4. Command-line arguments

## Extending Program.cs

Common extensions include:

### Database Integration
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Authentication
```csharp
builder.Services.AddAuthentication()
    .AddCookie(options => { /* cookie configuration */ });
```

### Logging
```csharp
builder.Logging.AddConsole();
builder.Logging.AddAzureWebAppDiagnostics();
```

### Custom Services
```csharp
builder.Services.AddScoped<IMyService, MyService>();
```

## Best Practices

1. **Keep it Simple**: Only add what you need
2. **Environment Specific**: Use environment checks for conditional configuration
3. **Service Lifetime**: Choose appropriate service lifetimes (Singleton, Scoped, Transient)
4. **Security First**: Always configure HTTPS and security headers for production
5. **Logging**: Configure appropriate logging levels for different environments

## Related Files

- `appsettings.json`: Base application configuration
- `appsettings.Development.json`: Development-specific overrides
- `Properties/launchSettings.json`: Development server configuration

This minimal yet powerful entry point provides a solid foundation for the web application while remaining easy to understand and extend.
