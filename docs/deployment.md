# Deployment Guide

This guide covers deployment procedures for MyWebApp, with a focus on Azure Container Apps (ACA) as the primary deployment target.

## 🎯 Deployment Overview

MyWebApp supports multiple deployment scenarios:

| Deployment Target | Complexity | Scalability | Cost | Recommended For |
|------------------|------------|-------------|------|-----------------|
| **Azure Container Apps** | Medium | Excellent | Variable | Production, auto-scaling |
| **Azure App Service** | Low | Good | Predictable | Small to medium apps |
| **Docker Container** | Medium | Good | Low | Development, testing |
| **IIS/Windows Server** | Low | Limited | Fixed | On-premises, legacy |

## 🚀 Azure Container Apps Deployment (Recommended)

Azure Container Apps is the recommended deployment target for modern, cloud-native applications.

### Prerequisites

- Azure subscription with appropriate permissions
- Azure CLI installed and configured
- Docker Desktop (for local container testing)
- Azure Container Registry (optional but recommended)

### Step 1: Setup Azure Resources

#### Install Azure CLI Extensions

```bash
# Install the containerapp extension
az extension add --name containerapp --upgrade

# Register required providers
az provider register --namespace Microsoft.App
az provider register --namespace Microsoft.OperationalInsights
```

#### Create Resource Group

```bash
# Set variables
$RESOURCE_GROUP="rg-mywebapp-prod"
$LOCATION="eastus"
$ENVIRONMENT="mywebapp-env"
$APP_NAME="mywebapp"

# Create resource group
az group create `
  --name $RESOURCE_GROUP `
  --location $LOCATION
```

#### Create Container Apps Environment

```bash
# Create the environment
az containerapp env create `
  --name $ENVIRONMENT `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION
```

### Step 2: Application Configuration

#### Create Dockerfile

Create a `Dockerfile` in the project root:

```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["MyWebApp.csproj", "./"]
RUN dotnet restore "MyWebApp.csproj"

# Copy source code
COPY . .
RUN dotnet build "MyWebApp.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MyWebApp.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser
USER appuser

# Copy published app
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080

# Start the application
ENTRYPOINT ["dotnet", "MyWebApp.dll"]
```

#### Update Program.cs for Container Deployment

Ensure your `Program.cs` is container-ready:

```csharp
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Configure for container deployment
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080); // Listen on port 8080 for container
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Remove HTTPS redirection for container deployment
// app.UseHttpsRedirection(); // Comment this out for container apps

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
```

### Step 3: Deploy to Azure Container Apps

#### Option A: Deploy from Local Docker Image

```bash
# Build and deploy directly
az containerapp up `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --environment $ENVIRONMENT `
  --source . `
  --target-port 8080 `
  --ingress external
```

#### Option B: Deploy from Azure Container Registry

```bash
# Create Azure Container Registry
$ACR_NAME="acrMyWebApp$(Get-Random)"
az acr create `
  --name $ACR_NAME `
  --resource-group $RESOURCE_GROUP `
  --sku Basic `
  --admin-enabled true

# Build and push image
az acr build `
  --registry $ACR_NAME `
  --image mywebapp:latest `
  .

# Deploy container app
az containerapp create `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --environment $ENVIRONMENT `
  --image "$ACR_NAME.azurecr.io/mywebapp:latest" `
  --target-port 8080 `
  --ingress external `
  --min-replicas 1 `
  --max-replicas 10 `
  --cpu 0.5 `
  --memory 1Gi
```

### Step 4: Configure Application Settings

```bash
# Set environment variables
az containerapp update `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --set-env-vars `
    "ASPNETCORE_ENVIRONMENT=Production" `
    "Logging__LogLevel__Default=Information"
```

### Step 5: Configure Custom Domain (Optional)

```bash
# Add custom domain
az containerapp hostname add `
  --hostname "mywebapp.contoso.com" `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP

# Bind SSL certificate
az containerapp ssl upload `
  --certificate-file "certificate.pfx" `
  --certificate-name "mywebapp-ssl" `
  --certificate-password "password" `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP
```

## 🔄 CI/CD Pipeline Setup

### GitHub Actions Workflow

Create `.github/workflows/deploy.yml`:

```yaml
name: Deploy to Azure Container Apps

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

env:
  AZURE_CONTAINER_APP_NAME: mywebapp
  AZURE_RESOURCE_GROUP: rg-mywebapp-prod
  AZURE_CONTAINER_ENVIRONMENT: mywebapp-env

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --no-restore --configuration Release
    
    - name: Test
      run: dotnet test --no-build --verbosity normal
    
    - name: Log in to Azure
      uses: azure/login@v1
      with:
        creds: ${{ secrets.AZURE_CREDENTIALS }}
    
    - name: Build and deploy Container App
      uses: azure/container-apps-deploy-action@v1
      with:
        appSourcePath: ${{ github.workspace }}
        acrName: ${{ secrets.ACR_NAME }}
        containerAppName: ${{ env.AZURE_CONTAINER_APP_NAME }}
        resourceGroup: ${{ env.AZURE_RESOURCE_GROUP }}
        containerAppEnvironment: ${{ env.AZURE_CONTAINER_ENVIRONMENT }}
        targetPort: 8080
        ingress: external
```

### Azure DevOps Pipeline

Create `azure-pipelines.yml`:

```yaml
trigger:
- main

variables:
  buildConfiguration: 'Release'
  azureSubscription: 'Azure-Service-Connection'
  containerAppName: 'mywebapp'
  resourceGroup: 'rg-mywebapp-prod'
  containerAppEnvironment: 'mywebapp-env'

stages:
- stage: Build
  displayName: 'Build Application'
  jobs:
  - job: Build
    displayName: 'Build job'
    pool:
      vmImage: 'ubuntu-latest'
    
    steps:
    - task: UseDotNet@2
      displayName: 'Use .NET 8.0'
      inputs:
        version: '8.0.x'
    
    - task: DotNetCoreCLI@2
      displayName: 'Restore packages'
      inputs:
        command: 'restore'
        projects: '**/*.csproj'
    
    - task: DotNetCoreCLI@2
      displayName: 'Build application'
      inputs:
        command: 'build'
        projects: '**/*.csproj'
        arguments: '--configuration $(buildConfiguration) --no-restore'
    
    - task: DotNetCoreCLI@2
      displayName: 'Run tests'
      inputs:
        command: 'test'
        projects: '**/*Tests.csproj'
        arguments: '--configuration $(buildConfiguration) --no-build'

- stage: Deploy
  displayName: 'Deploy to Azure Container Apps'
  dependsOn: Build
  condition: and(succeeded(), eq(variables['Build.SourceBranch'], 'refs/heads/main'))
  
  jobs:
  - deployment: Deploy
    displayName: 'Deploy job'
    pool:
      vmImage: 'ubuntu-latest'
    environment: 'production'
    
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureContainerApps@1
            displayName: 'Deploy to Container Apps'
            inputs:
              azureSubscription: $(azureSubscription)
              containerAppName: $(containerAppName)
              resourceGroup: $(resourceGroup)
              containerAppEnvironment: $(containerAppEnvironment)
              appSourcePath: '$(Pipeline.Workspace)'
              targetPort: 8080
              ingress: 'external'
```

## 🌐 Azure App Service Deployment

For simpler deployment scenarios, Azure App Service provides an easier option.

### Prerequisites

- Azure subscription
- Azure CLI or Visual Studio

### Deployment Steps

#### Using Azure CLI

```bash
# Create App Service Plan
az appservice plan create `
  --name "asp-mywebapp" `
  --resource-group $RESOURCE_GROUP `
  --sku B1 `
  --is-linux

# Create Web App
az webapp create `
  --name "mywebapp-$(Get-Random)" `
  --resource-group $RESOURCE_GROUP `
  --plan "asp-mywebapp" `
  --runtime "DOTNETCORE:8.0"

# Deploy from local directory
az webapp up `
  --name "mywebapp-$(Get-Random)" `
  --resource-group $RESOURCE_GROUP `
  --plan "asp-mywebapp"
```

#### Using Visual Studio

1. Right-click project → **Publish**
2. Choose **Azure** → **Azure App Service (Linux)**
3. Select subscription and create/select App Service
4. Configure deployment settings
5. Click **Publish**

## 🐳 Docker Container Deployment

For development and testing environments.

### Local Docker Deployment

```bash
# Build the image
docker build -t mywebapp:latest .

# Run the container
docker run -d `
  --name mywebapp `
  -p 8080:8080 `
  -e ASPNETCORE_ENVIRONMENT=Production `
  mywebapp:latest

# Access the application
# http://localhost:8080
```

### Docker Compose

Create `docker-compose.yml`:

```yaml
version: '3.8'

services:
  web:
    build: .
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - Logging__LogLevel__Default=Information
    restart: unless-stopped
    
  # Add database service when needed
  # database:
  #   image: mcr.microsoft.com/mssql/server:2022-latest
  #   environment:
  #     - ACCEPT_EULA=Y
  #     - SA_PASSWORD=YourPassword123!
  #   ports:
  #     - "1433:1433"
  #   volumes:
  #     - db_data:/var/opt/mssql

# volumes:
#   db_data:
```

Run with Docker Compose:

```bash
docker-compose up -d
```

## ⚙️ Environment Configuration

### Configuration Management

#### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production`, `Staging`, `Development` |
| `Logging__LogLevel__Default` | Default log level | `Information`, `Warning`, `Error` |
| `AllowedHosts` | Allowed hostnames | `mywebapp.com;*.mywebapp.com` |

#### Application Settings by Environment

**Production (`appsettings.Production.json`):**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "mywebapp.com;*.mywebapp.com"
}
```

**Staging (`appsettings.Staging.json`):**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "staging.mywebapp.com"
}
```

### Secrets Management

#### Azure Key Vault Integration

```csharp
// In Program.cs for production
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://mywebapp-keyvault.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

#### User Secrets (Development)

```bash
# Initialize user secrets
dotnet user-secrets init

# Add secret
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=..."
```

## 📊 Monitoring and Observability

### Application Insights Integration

#### Setup Application Insights

```bash
# Create Application Insights resource
az monitor app-insights component create `
  --app mywebapp-insights `
  --location $LOCATION `
  --resource-group $RESOURCE_GROUP `
  --application-type web
```

#### Configure Application Insights

Add to `Program.cs`:

```csharp
// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);
```

Add to `appsettings.json`:

```json
{
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=..."
  }
}
```

### Health Checks

Add health check endpoints:

```csharp
// In Program.cs
builder.Services.AddHealthChecks();

// Configure health check endpoint
app.MapHealthChecks("/health");
```

### Container Apps Monitoring

```bash
# View container app logs
az containerapp logs show `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --follow

# View container app metrics
az monitor metrics list `
  --resource "/subscriptions/{subscription}/resourceGroups/$RESOURCE_GROUP/providers/Microsoft.App/containerApps/$APP_NAME" `
  --metric "Requests"
```

## 🔐 Security Configuration

### Production Security Checklist

- [ ] HTTPS enforcement enabled
- [ ] HSTS headers configured
- [ ] Security headers implemented
- [ ] Sensitive data in Key Vault
- [ ] Container runs as non-root user
- [ ] Minimal base image used
- [ ] Regular security updates applied

### Security Headers

Add security middleware:

```csharp
// In Program.cs for production
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseSecurityHeaders(); // Custom middleware
}
```

Create security headers middleware:

```csharp
public static class SecurityHeadersExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
            context.Response.Headers.Add("X-Frame-Options", "DENY");
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
            
            await next();
        });
    }
}
```

## 🚨 Troubleshooting

### Common Deployment Issues

#### Container Won't Start

```bash
# Check container logs
az containerapp logs show --name $APP_NAME --resource-group $RESOURCE_GROUP

# Check container app status
az containerapp show --name $APP_NAME --resource-group $RESOURCE_GROUP --query "properties.runningStatus"
```

#### Port Configuration Issues

Ensure your application listens on the correct port (8080 for Container Apps):

```csharp
// In Program.cs
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});
```

#### Environment Variables Not Applied

```bash
# Check current environment variables
az containerapp show `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --query "properties.template.containers[0].env"
```

### Performance Issues

#### Scale Configuration

```bash
# Update scaling rules
az containerapp update `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP `
  --min-replicas 2 `
  --max-replicas 10 `
  --scale-rule-name "http-requests" `
  --scale-rule-type "http" `
  --scale-rule-metadata "concurrentRequests=10"
```

### Rollback Procedures

```bash
# List revisions
az containerapp revision list `
  --name $APP_NAME `
  --resource-group $RESOURCE_GROUP

# Activate previous revision
az containerapp revision activate `
  --revision "mywebapp--revision-name" `
  --resource-group $RESOURCE_GROUP
```

## 📋 Deployment Checklist

Before deploying to production:

### Pre-Deployment
- [ ] Code reviewed and approved
- [ ] All tests passing
- [ ] Security scan completed
- [ ] Performance testing done
- [ ] Configuration validated
- [ ] Secrets properly configured
- [ ] Database migrations ready (if applicable)

### Post-Deployment
- [ ] Application starts successfully
- [ ] Health checks passing
- [ ] Monitoring configured
- [ ] Logs are being generated
- [ ] Performance metrics normal
- [ ] SSL certificate valid
- [ ] Custom domain working (if configured)

## 🔗 Additional Resources

### Azure Documentation
- [Azure Container Apps Documentation](https://docs.microsoft.com/azure/container-apps/)
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/)

### Best Practices
- [ASP.NET Core Deployment Best Practices](https://docs.microsoft.com/aspnet/core/host-and-deploy/)
- [Docker Best Practices](https://docs.docker.com/develop/dev-best-practices/)
- [Container Security Best Practices](https://docs.microsoft.com/azure/security/fundamentals/container-security)

---

🚀 **Ready to deploy your application to the cloud!**