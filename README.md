# MyWebApp - ASP.NET Core Web Application

[![.NET](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-green.svg)](https://docs.microsoft.com/aspnet/core)

A modern ASP.NET Core 8.0 web application built using Razor Pages architecture, designed for deployment to Azure Container Apps (ACA).

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Running the Application](#running-the-application)
- [Project Structure](#project-structure)
- [Architecture](#architecture)
- [Configuration](#configuration)
- [Development](#development)
- [Deployment](#deployment)
- [Documentation](#documentation)
- [Contributing](#contributing)
- [License](#license)

## 🚀 Overview

MyWebApp is a clean, minimal ASP.NET Core web application that serves as a foundation for building modern web applications. It follows ASP.NET Core best practices and is optimized for containerized deployment scenarios, particularly Azure Container Apps.

### Key Characteristics

- **Framework**: ASP.NET Core 8.0
- **Architecture**: Razor Pages with Model-View-Controller pattern
- **UI Framework**: Bootstrap 5.x with custom styling
- **Target Platform**: Cross-platform (.NET 8.0)
- **Deployment Target**: Azure Container Apps (ACA)

## ✨ Features

- 🏠 **Home Page**: Welcome landing page with navigation
- 📋 **Todo Management**: Complete task management system with create, update, delete, and completion tracking
- 🔒 **Privacy Policy**: Dedicated privacy policy page
- ❌ **Error Handling**: Custom error pages with user-friendly messages
- 📱 **Responsive Design**: Mobile-first Bootstrap-based UI
- 🔧 **Development Tools**: Hot reload, debugging support
- 🚀 **Production Ready**: HSTS, HTTPS redirection, static file optimization
- 📦 **Container Ready**: Optimized for containerized deployment

## 🏁 Getting Started

### Prerequisites

Before running this application, ensure you have the following installed:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/) for version control

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MyWebAppACA
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the application**
   ```bash
   dotnet build
   ```

### Running the Application

#### Development Mode
```bash
dotnet run
```
The application will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

#### Production Mode
```bash
dotnet run --environment Production
```

#### Using Visual Studio
1. Open `MyWebApp.sln` in Visual Studio
2. Press `F5` or click the "Start" button
3. The application will launch in your default browser

## 📁 Project Structure

```
MyWebAppACA/
├── 📄 Program.cs                    # Application entry point
├── 📄 MyWebApp.csproj              # Project configuration
├── 📄 MyWebApp.sln                 # Solution file
├── 📄 appsettings.json             # Application configuration
├── 📄 appsettings.Development.json # Development-specific settings
├── 📁 Pages/                       # Razor Pages
│   ├── 📄 Index.cshtml             # Home page view
│   ├── 📄 Index.cshtml.cs          # Home page model
│   ├── 📄 Privacy.cshtml           # Privacy page view
│   ├── 📄 Privacy.cshtml.cs        # Privacy page model
│   ├── 📄 Contact.cshtml           # Contact page view
│   ├── 📄 Contact.cshtml.cs        # Contact page model
│   ├── 📄 Error.cshtml             # Error page view
│   ├── 📄 Error.cshtml.cs          # Error page model
│   └── 📁 Shared/                  # Shared views and layouts
│       ├── 📄 _Layout.cshtml       # Main layout template
│       ├── 📄 _Layout.cshtml.css   # Layout-specific styles
│       └── 📄 _ValidationScriptsPartial.cshtml
├── 📁 wwwroot/                     # Static web assets
│   ├── 📄 favicon.ico              # Site icon
│   ├── 📁 css/                     # Stylesheets
│   │   └── 📄 site.css             # Custom application styles
│   ├── 📁 js/                      # JavaScript files
│   │   └── 📄 site.js              # Custom application scripts
│   └── 📁 lib/                     # Client-side libraries
│       ├── 📁 bootstrap/           # Bootstrap framework
│       ├── 📁 jquery/              # jQuery library
│       └── 📁 jquery-validation/   # jQuery validation
├── 📁 Properties/                  # Application properties
│   └── 📄 launchSettings.json      # Development launch settings
└── 📁 docs/                       # Project documentation
    ├── 📄 README.md               # Project documentation index
    ├── 📄 development.md          # Development guidelines
    ├── 📄 deployment.md           # Deployment procedures
    └── 📄 architecture.md         # Architecture documentation
```

## 🏗️ Architecture

MyWebApp follows the **Razor Pages** architectural pattern, which provides a page-focused approach ideal for scenarios where the UI is read-heavy and form-based.

### Core Components

1. **Program.cs**: Application bootstrap and configuration
2. **Razor Pages**: Page-centric UI components with associated models
3. **Shared Layout**: Common UI structure and navigation
4. **Static Assets**: CSS, JavaScript, and other web resources
5. **Configuration System**: Environment-specific settings management

For detailed architecture information, see [docs/architecture.md](docs/architecture.md).

## ⚙️ Configuration

The application uses the standard ASP.NET Core configuration system:

- `appsettings.json`: Base application settings
- `appsettings.Development.json`: Development environment overrides
- `launchSettings.json`: Development server configuration

### Key Configuration Areas

- **Logging**: Configurable log levels for different components
- **Allowed Hosts**: Security configuration for allowed request hosts
- **Development Settings**: Hot reload, detailed errors, etc.

## 🛠️ Development

For comprehensive development guidelines, see [docs/development.md](docs/development.md).

### Quick Development Commands

```bash
# Run in development mode with hot reload
dotnet watch run

# Run tests (when available)
dotnet test

# Create a new Razor page
dotnet new page -n NewPage -o Pages

# Check for outdated packages
dotnet list package --outdated
```

### Development Tools

- **Hot Reload**: Automatic refresh during development
- **Browser Refresh**: Automatic browser refresh on file changes
- **Exception Pages**: Detailed error information in development
- **Static File Serving**: Efficient serving of CSS, JS, and images

## 🚀 Deployment

This application is designed for deployment to **Azure Container Apps**. For detailed deployment instructions, see [docs/deployment.md](docs/deployment.md).

### Deployment Options

1. **Azure Container Apps** (Recommended)
2. **Azure App Service**
3. **Docker Container**
4. **Traditional IIS Hosting**

### Environment Configuration

- **Development**: Local development with debugging enabled
- **Staging**: Pre-production testing environment
- **Production**: Live production environment with optimizations

## 📚 Documentation

Detailed documentation is organized in the `docs/` folder:

| Document | Description |
|----------|-------------|
| [📖 Architecture](docs/architecture.md) | Application architecture and design patterns |
| [🛠️ Development](docs/development.md) | Development setup, guidelines, and best practices |
| [🚀 Deployment](docs/deployment.md) | Deployment procedures and environment configuration |
| [📄 Program.cs](docs/Program.md) | Application entry point documentation |
| [📋 Todos](docs/Todos.md) | Todo management feature documentation |
| [🏠 Pages](docs/) | Individual page documentation |
| [🎨 Styling](docs/SiteCSS.md) | CSS and styling guidelines |
| [⚡ JavaScript](docs/SiteJS.md) | Client-side scripting documentation |

## 🤝 Contributing

We welcome contributions to improve this application! Please follow these guidelines:

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

### Development Guidelines

- Follow established coding conventions
- Add appropriate documentation for new features
- Ensure all tests pass before submitting
- Update relevant documentation

## 📞 Support

For support and questions:

- 📧 **Issues**: Use GitHub Issues for bug reports and feature requests
- 📖 **Documentation**: Check the `docs/` folder for detailed information
- 🌐 **ASP.NET Core Docs**: [Official Microsoft Documentation](https://docs.microsoft.com/aspnet/core)

## 🔗 Additional Resources

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Razor Pages Documentation](https://docs.microsoft.com/aspnet/core/razor-pages)
- [Azure Container Apps Documentation](https://docs.microsoft.com/azure/container-apps)
- [.NET 8.0 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-8)

---

**Built with ❤️ using ASP.NET Core 8.0**