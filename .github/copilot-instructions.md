# SignalR MSAL Demo - .NET 9 Blazor Server with Azure SignalR

This is a demo application for exploring options for calling downstream APIs from a .NET 9 Blazor Server application that uses Azure SignalR Service and Microsoft Authentication Library (MSAL) for authentication.

**Always reference these instructions first and fallback to search or bash commands only when you encounter unexpected information that does not match the info here.**

## Environment Setup

### .NET 9 Installation
- **CRITICAL**: This project targets .NET 9. The environment currently has .NET 8.0.118 installed.
- Install .NET 9 SDK before proceeding:
  ```bash
  # Download and install .NET 9 SDK using official installer
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 9.0
  export DOTNET_ROOT=$HOME/.dotnet
  export PATH=$PATH:$HOME/.dotnet
  ```
- Verify installation: `dotnet --version` should show 9.0.x
- **NOTE**: Latest available version is 9.0.303 (as of validation date)

### Required SDKs and Tools
- .NET 9 SDK (see above)
- Azure CLI (for Azure SignalR setup): `curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash`
- Azure SignalR Service connection (configuration required)

## Project Structure and Key Commands

### Working with Existing Projects
The repository contains a complete solution with three projects:
- **WebApp** - .NET 9 Blazor Server application with Azure SignalR and MSAL
- **APINoGraph** - Web API project for downstream API calls without Microsoft Graph
- **APIWithGraph** - Web API project with Microsoft Graph integration

All projects are already configured with the necessary packages and dependencies.

### Build Commands
- **NEVER CANCEL BUILD COMMANDS** - Wait for completion even if they appear slow.
- `dotnet restore` -- Takes 5-15 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- `dotnet build` -- Takes 10-20 seconds for clean build. NEVER CANCEL. Set timeout to 60+ seconds.
- `dotnet build --configuration Release` -- Takes 15-30 seconds. NEVER CANCEL. Set timeout to 90+ seconds.

### Running the Application
- `dotnet run --project WebApp` -- Starts the Blazor Server app on localhost:5000 or localhost:5001 (HTTPS)
- `dotnet run --project WebApp --environment Development` -- Run with development settings
- `dotnet run --project WebApp --configuration Release` -- Run optimized release build
- `dotnet run --project APINoGraph` -- Start the API without Graph integration (typically on localhost:5002)
- `dotnet run --project APIWithGraph` -- Start the API with Graph integration (typically on localhost:5003)
- The application will show: "Now listening on: http://localhost:xxxx"
- **CRITICAL**: WebApp requires Azure SignalR connection string in appsettings.json or user secrets

### Testing
- `dotnet test` -- Runs all tests in the solution. Takes 1-5 seconds if no tests exist, longer with actual tests. NEVER CANCEL. Set timeout to 30+ minutes for comprehensive test suites.
- Test projects can be added as: `dotnet new xunit -n SignalRMSALDemo.Tests`
- Integration tests may require Azure resources and take significantly longer.

### Code Quality and Linting
- `dotnet format` -- Takes 10-15 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- `dotnet format --verify-no-changes` -- Verify formatting without making changes
- **ALWAYS run `dotnet format` before committing changes**

## Validation Scenarios

After making any changes, **ALWAYS** run through these validation steps:

### Basic Application Validation
1. `dotnet build` -- Ensure clean build of entire solution
2. `dotnet run --project WebApp` -- Start the Blazor Server application
3. Navigate to https://localhost:5001 or displayed URL
4. Verify the Blazor application loads with navigation menu
5. Test at least one interactive component (counter, weather data, etc.)
6. Stop the application (Ctrl+C)

### Authentication Flow Validation (When MSAL is configured)
1. Start the application: `dotnet run --project WebApp`
2. Click "Login" or authentication link
3. Complete Microsoft authentication flow
4. Verify authenticated user state is displayed
5. Test accessing a protected page/component
6. Test logout functionality

### SignalR Functionality Validation (When configured)
1. Start the application with valid Azure SignalR connection: `dotnet run --project WebApp`
2. Open browser developer tools and check Console tab
3. Verify SignalR connection is established (check for connection messages)
4. Test real-time features (if implemented)
5. Verify no SignalR connection errors in console

### End-to-End API Call Validation (When downstream APIs are configured)
1. Start the WebApp: `dotnet run --project WebApp`
2. Start the API projects: `dotnet run --project APINoGraph` and/or `dotnet run --project APIWithGraph`
3. Authenticate as a valid user in the WebApp
4. Navigate to functionality that calls downstream APIs
5. Verify API calls complete successfully
6. Check browser network tab for proper authentication headers
7. Verify API responses are displayed correctly in the UI

## Common File Locations and Navigation

### Key Project Files
- `WebApp/Program.cs` -- Blazor Server application startup and configuration
- `WebApp/appsettings.json` / `WebApp/appsettings.Development.json` -- WebApp configuration settings
- `WebApp/Components/App.razor` -- Root application component
- `WebApp/Components/Layout/MainLayout.razor` -- Main layout template
- `WebApp/Components/Pages/` -- Page components
- `WebApp/wwwroot/` -- Static web assets (CSS, JS, images)
- `APINoGraph/Program.cs` -- API without Graph startup configuration
- `APIWithGraph/Program.cs` -- API with Graph startup configuration

### Configuration Files
- Azure SignalR connection string: Store in WebApp user secrets or WebApp/appsettings.json
- MSAL configuration: Typically in WebApp/appsettings.json under "AzureAd" section
- API configurations: In respective API project appsettings.json files
- **NEVER commit connection strings or secrets to source control**

### Frequently Modified Files
- When adding new pages: Create .razor files in `WebApp/Components/Pages/`
- When adding SignalR hubs: Create in `WebApp/Hubs/` directory
- When adding API controllers: Add to `APINoGraph/Controllers/` or `APIWithGraph/Controllers/`
- When adding services: Add to `WebApp/Services/` directory
- **Always check these locations when working on new features**

## Repository File Structure
```
/
├── .github/
│   └── copilot-instructions.md (this file)
├── .gitignore
├── README.md
├── SignalRMSALDemo.sln (solution file)
├── WebApp/ (main Blazor Server application)
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   ├── WebApp.csproj
│   ├── Components/
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   ├── Layout/
│   │   │   └── MainLayout.razor
│   │   └── Pages/
│   │       ├── Home.razor
│   │       └── Counter.razor
│   ├── Properties/
│   │   └── launchSettings.json
│   ├── Services/ (service classes)
│   ├── Hubs/ (create for SignalR hubs)
│   └── wwwroot/
│       ├── css/
│       └── js/
├── APINoGraph/ (API project without Graph)
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Program.cs
│   ├── APINoGraph.csproj
│   ├── APINoGraph.http
│   ├── Controllers/
│   └── Properties/
└── APIWithGraph/ (API project with Graph integration)
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Program.cs
    ├── APIWithGraph.csproj
    ├── APIWithGraph.http
    ├── Controllers/
    └── Properties/
```

## Azure Services Configuration

### Azure SignalR Service
- Connection string format: `Endpoint=https://[servicename].service.signalr.net;AccessKey=[key];Version=1.0;`
- Configure in `WebApp/Program.cs`: `builder.Services.AddSignalR().AddAzureSignalR(connectionString);`

### MSAL Configuration
- Requires Azure AD app registration
- Configuration keys: TenantId, ClientId, Instance
- Configure in `WebApp/Program.cs`: `builder.Services.AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));`

## Package Management and Dependencies

### Core Packages (Validated Working)
The projects already include these packages:
- `Microsoft.Azure.SignalR 1.21.0` -- Azure SignalR Service integration (WebApp)
- `Microsoft.Identity.Web 2.15.1` -- MSAL authentication for web apps (WebApp)
- `Microsoft.Identity.Web.UI 2.15.0` -- UI components for authentication (WebApp)
- `Microsoft.Graph` -- Microsoft Graph SDK (APIWithGraph)

### Adding Packages
- Always use specific versions: `dotnet add package [PackageName] --version [Version]`
- Specify target project: `dotnet add WebApp package [PackageName] --version [Version]`
- Package restore takes 10-15 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- After adding packages, always run `dotnet build` to verify compatibility

### Common Development Workflow
1. `dotnet restore` -- Restore packages if needed
2. Make code changes (in WebApp, APINoGraph, or APIWithGraph)
3. `dotnet build` -- Verify compilation (10-20 seconds)
4. `dotnet run --project WebApp` -- Test the Blazor application
5. `dotnet format` -- Format code before committing (10-15 seconds)
6. Commit changes

### Watch Mode for Development
- Use `dotnet watch run --project WebApp` for automatic rebuilds during development
- Use `dotnet watch run --project APINoGraph` for API development with auto-reload
- Use `dotnet watch run --project APIWithGraph` for Graph API development with auto-reload
- Watch mode detects file changes and rebuilds automatically
- Faster feedback loop than manual build/run cycles

## Troubleshooting Common Issues

### Build Issues
- **PackageReference errors**: Run `dotnet restore` to restore packages
- **Framework targeting errors**: Ensure `<TargetFramework>net9.0</TargetFramework>` in .csproj files
- **Missing Azure packages**: Check that packages are referenced in the correct project files

### Runtime Issues
- **SignalR connection failures**: Verify Azure SignalR connection string is correct and accessible in WebApp configuration
- **Authentication errors**: Check Azure AD app registration and MSAL configuration in WebApp
- **API communication errors**: Ensure API projects are running and accessible from WebApp
- **Port conflicts**: Applications may start on different ports if default ports are in use

### Performance Validation
- **First run is slower**: Package restoration and first-time compilation add overhead
- **Subsequent builds are faster**: Incremental compilation reduces build time to 5-10 seconds
- **Watch mode**: Use `dotnet watch run` for automatic rebuilds during development

## CRITICAL Reminders

- **NEVER CANCEL**: Build operations may take 30+ seconds, tests may take minutes. Always use appropriate timeouts (60+ seconds for builds, 30+ minutes for comprehensive test suites).
- **Always validate manually**: After code changes, run the application and test user scenarios - don't just rely on successful compilation.
- **Azure dependencies**: The WebApp requires Azure SignalR Service connection and Azure AD configuration to function fully. API projects may have their own Azure dependencies.
- **Security**: Never commit connection strings, secrets, or authentication keys to source control. Use user secrets or environment variables.

## Development Tips

### User Secrets Management
- Store sensitive configuration in user secrets for each project:
  - `dotnet user-secrets init --project WebApp`
  - `dotnet user-secrets init --project APINoGraph`
  - `dotnet user-secrets init --project APIWithGraph`
- Add connection strings: `dotnet user-secrets set "Azure:SignalR:ConnectionString" "[connection-string]" --project WebApp`
- Add Azure AD config: `dotnet user-secrets set "AzureAd:TenantId" "[tenant-id]" --project WebApp`

### Hot Reload and Fast Development
- Use `dotnet watch run --project WebApp` for automatic rebuilds with hot reload support
- Use `dotnet watch run --project APINoGraph` for API development with hot reload
- Use `dotnet watch run --project APIWithGraph` for Graph API development with hot reload
- Hot reload works for most Razor and C# changes without full restart
- Press "Ctrl + R" in watch mode to force restart if needed

### Performance Optimization
- First build after adding packages is slower (15-30 seconds)
- Subsequent builds are much faster (2-5 seconds)
- Release builds take longer but produce optimized output: `dotnet build --configuration Release`

### Common Command Reference
```bash
# Project-specific build and run commands
dotnet build                                 # Build entire solution (15-25s)
dotnet run --project WebApp                 # Start Blazor Server app
dotnet run --project APINoGraph             # Start API without Graph
dotnet run --project APIWithGraph           # Start API with Graph integration

# Daily development workflow  
dotnet restore                               # If packages are out of sync
dotnet build                                # Verify compilation (10-20s)
dotnet watch run --project WebApp           # Development with hot reload
dotnet format                               # Format code before commit (10-15s)
dotnet test                                 # Run tests when they exist

# Package management
dotnet add WebApp package [Name] --version [Version]      # Add package to WebApp
dotnet add APINoGraph package [Name] --version [Version]  # Add package to API
dotnet list package                                      # List installed packages
dotnet restore                                          # Restore packages if needed

# User secrets (per project)
dotnet user-secrets init --project WebApp               # Initialize secrets for WebApp
dotnet user-secrets set "Key:Value" "[value]" --project WebApp  # Set secret for WebApp
```