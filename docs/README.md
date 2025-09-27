# MyWebApp Documentation

## Overview
MyWebApp is an ASP.NET Core web application using Razor Pages. It provides a simple structure for building web apps with .NET 8.0, including home, error, and privacy pages, a shared layout, and static assets (CSS/JS).

## Project Structure
- **Program.cs**: Configures and runs the web server.
- **MyWebApp.csproj**: Project settings and .NET version.
- **Pages/**: Contains Razor Pages for the site (Index, Error, Privacy) and shared layout files.
- **wwwroot/**: Static files (CSS, JS, images, libraries).
- **Properties/**: Launch settings for development.

## How to Use
- Start the app with `dotnet run`.
- Access pages via `/`, `/Error`, `/Privacy`.
- Customize layout and styles in `Pages/Shared/_Layout.cshtml` and `wwwroot/css/site.css`.

## Documentation
Individual documentation for each file is available in the `docs/` folder.

## References
- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Bundling and Minification](https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification)
