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

### Initial Project Creation
When creating the initial Blazor Server project structure:
```bash
# Create solution and main project
dotnet new sln -n SignalrMsalDemo
dotnet new blazor -n SignalrMsalDemo.Web --framework net9.0
dotnet sln add SignalrMsalDemo.Web

# Add Azure SignalR and MSAL packages
cd SignalrMsalDemo.Web
dotnet add package Microsoft.Azure.SignalR --version 1.21.0
dotnet add package Microsoft.Identity.Web --version 2.15.0
dotnet add package Microsoft.Identity.Web.UI --version 2.15.0
```

### Build Commands
- **NEVER CANCEL BUILD COMMANDS** - Wait for completion even if they appear slow.
- `dotnet restore` -- Takes 5-15 seconds. NEVER CANCEL. Set timeout to 30+ seconds.
- `dotnet build` -- Takes 10-20 seconds for clean build. NEVER CANCEL. Set timeout to 60+ seconds.
- `dotnet build --configuration Release` -- Takes 15-30 seconds. NEVER CANCEL. Set timeout to 90+ seconds.

### Running the Application
- `dotnet run` -- Starts the Blazor Server app on localhost:5000 or localhost:5001 (HTTPS)
- `dotnet run --environment Development` -- Run with development settings
- `dotnet run --configuration Release` -- Run optimized release build
- The application will show: "Now listening on: http://localhost:xxxx"
- **CRITICAL**: Application requires Azure SignalR connection string in appsettings.json or user secrets

### Testing
- `dotnet test` -- Takes 1-5 seconds if no tests exist, longer with actual tests. NEVER CANCEL. Set timeout to 30+ minutes for comprehensive test suites.
- Test projects should be created as: `dotnet new xunit -n SignalrMsalDemo.Tests`
- Integration tests may require Azure resources and take significantly longer.

### Code Quality and Linting
- `dotnet format` -- Takes 10-15 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- `dotnet format --verify-no-changes` -- Verify formatting without making changes
- **ALWAYS run `dotnet format` before committing changes**

## Validation Scenarios

After making any changes, **ALWAYS** run through these validation steps:

### Basic Application Validation
1. `dotnet build` -- Ensure clean build
2. `dotnet run` -- Start the application
3. Navigate to https://localhost:5001 or displayed URL
4. Verify the Blazor application loads with navigation menu
5. Test at least one interactive component (counter, weather data, etc.)
6. Stop the application (Ctrl+C)

### Authentication Flow Validation (When MSAL is configured)
1. Start the application: `dotnet run`
2. Click "Login" or authentication link
3. Complete Microsoft authentication flow
4. Verify authenticated user state is displayed
5. Test accessing a protected page/component
6. Test logout functionality

### SignalR Functionality Validation (When configured)
1. Start the application with valid Azure SignalR connection
2. Open browser developer tools and check Console tab
3. Verify SignalR connection is established (check for connection messages)
4. Test real-time features (if implemented)
5. Verify no SignalR connection errors in console

### End-to-End API Call Validation (When downstream APIs are configured)
1. Authenticate as a valid user
2. Navigate to functionality that calls downstream APIs
3. Verify API calls complete successfully
4. Check browser network tab for proper authentication headers
5. Verify API responses are displayed correctly in the UI

## Common File Locations and Navigation

### Key Project Files
- `Program.cs` -- Application startup and configuration
- `appsettings.json` / `appsettings.Development.json` -- Configuration settings
- `Components/App.razor` -- Root application component
- `Components/Layout/MainLayout.razor` -- Main layout template
- `Components/Pages/` -- Page components
- `wwwroot/` -- Static web assets (CSS, JS, images)

### Configuration Files
- Azure SignalR connection string: Store in user secrets or appsettings.json
- MSAL configuration: Typically in appsettings.json under "AzureAd" section
- **NEVER commit connection strings or secrets to source control**

### Frequently Modified Files
- When adding new pages: Create .razor files in `Components/Pages/`
- When adding SignalR hubs: Create in `Hubs/` directory
- When adding API controllers: Create in `Controllers/` directory (if using MVC pattern)
- **Always check these locations when working on new features**

## Repository File Structure
```
/
├── .github/
│   └── copilot-instructions.md (this file)
├── .gitignore
├── README.md
├── SignalrMsalDemo.sln (when created)
└── SignalrMsalDemo.Web/ (main project when created)
    ├── appsettings.json
    ├── appsettings.Development.json
    ├── Program.cs
    ├── SignalrMsalDemo.Web.csproj
    ├── Components/
    │   ├── App.razor
    │   ├── Routes.razor
    │   ├── Layout/
    │   │   └── MainLayout.razor
    │   └── Pages/
    │       ├── Home.razor
    │       └── Counter.razor
    ├── Properties/
    │   └── launchSettings.json
    ├── Hubs/ (create for SignalR hubs)
    └── wwwroot/
        ├── css/
        └── js/
```

## Azure Services Configuration

### Azure SignalR Service
- Connection string format: `Endpoint=https://[servicename].service.signalr.net;AccessKey=[key];Version=1.0;`
- Configure in `Program.cs`: `builder.Services.AddSignalR().AddAzureSignalR(connectionString);`

### MSAL Configuration
- Requires Azure AD app registration
- Configuration keys: TenantId, ClientId, Instance
- Configure in `Program.cs`: `builder.Services.AddMicrosoftIdentityWebApp(configuration.GetSection("AzureAd"));`

## Package Management and Dependencies

### Core Packages (Validated Working)
- `Microsoft.Azure.SignalR 1.21.0` -- Azure SignalR Service integration
- `Microsoft.Identity.Web 2.15.1` -- MSAL authentication for web apps
- `Microsoft.Identity.Web.UI 2.15.0` -- UI components for authentication

### Adding Packages
- Always use specific versions: `dotnet add package [PackageName] --version [Version]`
- Package restore takes 10-15 seconds. NEVER CANCEL. Set timeout to 60+ seconds.
- After adding packages, always run `dotnet build` to verify compatibility

### Common Development Workflow
1. `dotnet restore` -- Restore packages if needed
2. Make code changes
3. `dotnet build` -- Verify compilation (10-20 seconds)
4. `dotnet run` -- Test the application
5. `dotnet format` -- Format code before committing (10-15 seconds)
6. Commit changes

### Watch Mode for Development
- Use `dotnet watch run` for automatic rebuilds during development
- Watch mode detects file changes and rebuilds automatically
- Faster feedback loop than manual build/run cycles

## Troubleshooting Common Issues

### Build Issues
- **PackageReference errors**: Run `dotnet restore` to restore packages
- **Framework targeting errors**: Ensure `<TargetFramework>net9.0</TargetFramework>` in .csproj
- **Missing Azure packages**: Add required Azure SignalR and MSAL packages (see setup commands above)

### Runtime Issues
- **SignalR connection failures**: Verify Azure SignalR connection string is correct and accessible
- **Authentication errors**: Check Azure AD app registration and MSAL configuration
- **Port conflicts**: Application may start on different port if 5000/5001 are in use

### Performance Validation
- **First run is slower**: Package restoration and first-time compilation add overhead
- **Subsequent builds are faster**: Incremental compilation reduces build time to 5-10 seconds
- **Watch mode**: Use `dotnet watch run` for automatic rebuilds during development

## CRITICAL Reminders

- **NEVER CANCEL**: Build operations may take 30+ seconds, tests may take minutes. Always use appropriate timeouts (60+ seconds for builds, 30+ minutes for comprehensive test suites).
- **Always validate manually**: After code changes, run the application and test user scenarios - don't just rely on successful compilation.
- **Azure dependencies**: This application requires Azure SignalR Service connection and Azure AD configuration to function fully.
- **Security**: Never commit connection strings, secrets, or authentication keys to source control. Use user secrets or environment variables.

## Development Tips

### User Secrets Management
- Store sensitive configuration in user secrets: `dotnet user-secrets init`
- Add connection strings: `dotnet user-secrets set "Azure:SignalR:ConnectionString" "[connection-string]"`
- Add Azure AD config: `dotnet user-secrets set "AzureAd:TenantId" "[tenant-id]"`

### Hot Reload and Fast Development
- Use `dotnet watch run` for automatic rebuilds with hot reload support
- Hot reload works for most Razor and C# changes without full restart
- Press "Ctrl + R" in watch mode to force restart if needed

### Performance Optimization
- First build after adding packages is slower (15-30 seconds)
- Subsequent builds are much faster (2-5 seconds)
- Release builds take longer but produce optimized output: `dotnet build --configuration Release`

### Common Command Reference
```bash
# Initial setup (run once)
dotnet new sln -n SignalrMsalDemo
dotnet new blazor -n SignalrMsalDemo.Web --framework net9.0
dotnet sln add SignalrMsalDemo.Web

# Daily development workflow  
dotnet restore                    # If packages are out of sync
dotnet build                      # Verify compilation (10-20s)
dotnet run                        # Start application
dotnet watch run                  # Development with hot reload
dotnet format                     # Format code before commit (10-15s)
dotnet test                       # Run tests when they exist

# Package management
dotnet add package [Name] --version [Version]   # Add specific package version
dotnet list package                            # List installed packages
dotnet restore                                # Restore packages if needed
```