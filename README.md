# SignalR MSAL Demo - .NET 9 Blazor Server with Azure SignalR

This repository demonstrates various approaches for calling downstream APIs from a .NET 9 Blazor Server application that uses Azure SignalR Service and Microsoft Authentication Library (MSAL) for authentication.

## Sample Code

This repository contains sample code intended for demonstration purposes. The
code is provided as-is and may require modifications to fit specific use cases or
production environments.

## Repository Overview

This solution contains three main projects that demonstrate different authentication patterns and API calling techniques:

### APINoGraph - App Roles Authentication
A Web API project that **does not** use Microsoft Graph and is designed for app-to-app authentication scenarios.

**Key Features:**
- Uses app roles for authorization via `[Authorize(Roles = "APINoGraph.ApiAccess")]`
- Secured with Azure AD app roles configuration
- Called using **ForApp** authentication patterns (application permissions)
- Simple `/api/hello` endpoint that returns a greeting message

**Authentication Flow:**
- The web app acquires tokens using application permissions (client credentials flow)
- Tokens are validated against configured app roles in Azure AD
- No user context required for API access

### APIWithGraph - Scopes-Based Authentication  
A Web API project that **integrates with Microsoft Graph** and requires user context.

**Key Features:**
- Uses scopes for authorization via `[AuthorizeForScopes(Scopes = new[] { "User.Read" })]`
- Integrates with Microsoft Graph to access user profile information
- Called using **ForUser** authentication patterns (delegated permissions)
- `/api/profile/mobile-greeting` endpoint that retrieves user's name and mobile phone from Graph

**Authentication Flow:**
- The web app acquires tokens on behalf of the signed-in user (OAuth 2.0 on-behalf-of flow)
- Tokens include User.Read scope to access Microsoft Graph
- API calls Microsoft Graph to retrieve user profile data
- Requires user consent for Graph permissions

### WebApp - Blazor Server with Three API Calling Techniques

The main Blazor Server application demonstrates **three different approaches** for calling downstream APIs:

#### 1. Manual Token Acquisition and Header Injection
**Implementation:** `ApiService.cs`

```csharp
// For APINoGraph (ForApp)
var accessToken = await _tokenAcquisition.GetAccessTokenForAppAsync(scopes[0]);
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", accessToken);

// For APIWithGraph (ForUser)  
var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scopes);
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", accessToken);
```

**Features:**
- Direct control over token acquisition and HTTP header management
- Uses `ITokenAcquisition` service from Microsoft.Identity.Web
- Explicit error handling for token acquisition failures
- Manual Bearer token injection into HTTP requests

#### 2. IDownstreamApi Service Usage
**Implementation:** `DownstreamApiService.cs`

```csharp
// For APINoGraph (ForApp)
var result = await _downstreamApi.CallApiForAppAsync("APINoGraph", options =>
{
    options.RelativePath = "api/hello";
});

// For APIWithGraph (ForUser)
var result = await _downstreamApi.CallApiForUserAsync("APIWithGraph", options =>
{
    options.RelativePath = "api/profile/mobile-greeting";
});
```

**Features:**
- Built-in Microsoft.Identity.Web abstraction
- Automatic token acquisition and injection
- Simplified configuration through `AddDownstreamApi()` in Program.cs

#### 3. Custom MessageHandler Approach  
**Implementation:** `ApiServiceWithHandler.cs` + `AuthorizationHeaderAppHttpHandler.cs` + `AuthorizationHeaderUserHttpHandler.cs`

```csharp
// HttpClient configuration with message handlers
builder.Services.AddHttpClient("APINoGraphHandler", client =>
{
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<AuthorizationHeaderAppHttpHandler>();

builder.Services.AddHttpClient("APIWithGraphHandler", client =>
{
    client.BaseAddress = new Uri(baseUrl);
}).AddHttpMessageHandler<AuthorizationHeaderUserHttpHandler>();
```

**Features:**
- Automatic token injection via HTTP message handlers
- Clean separation of concerns - authentication logic isolated in handlers
- Works transparently with HttpClientFactory
- No manual token management required in business logic

## API Demo Page

The `/api-demo` page provides an interactive interface to test all three API calling techniques:

**APINoGraph Testing:**
- "Call APINoGraph (Manual Token)" - Tests manual token acquisition approach
- "Call APINoGraph (DownstreamApi ForApp)" - Tests IDownstreamApi approach  
- "Call APINoGraph (MessageHandler)" - Tests custom message handler approach

**APIWithGraph Testing:**  
- "Call APIWithGraph (Manual Token)" - Tests manual token acquisition approach
- "Call APIWithGraph (DownstreamApi ForUser)" - Tests IDownstreamApi approach
- "Call APIWithGraph (MessageHandler)" - Tests custom message handler approach

Each button demonstrates the corresponding authentication technique and displays the API response or error messages.

## Infrastructure Components

### Redis Distributed Token Cache
The web application uses **Redis for distributed token caching** to improve performance and enable horizontal scaling:

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// Token cache configuration
.AddDistributedTokenCaches();
```

**Benefits:**
- Shared token cache across multiple application instances
- Improved performance by avoiding repeated token acquisition
- Supports scaling scenarios with multiple web app instances

### Azure SignalR Service
The application integrates **Azure SignalR Service** for real-time communication capabilities:

```csharp
builder.Services.AddSignalR().AddAzureSignalR(
    builder.Configuration.GetConnectionString("SignalR")
);
```

**Features:**
- Scalable real-time communication infrastructure
- WebSocket fallback support
- Integrated with Blazor Server's built-in real-time capabilities

## Known Limitations

⚠️ **MessageHandler + Azure SignalR Compatibility Issue**

There is a known issue where the **MessageHandler approach does not work correctly when used together with Azure SignalR Service**. All other authentication mechanisms work properly with Azure SignalR.

**Affected Scenario:**
- Custom MessageHandler approach (`AuthorizationHeaderAppHttpHandler` / `AuthorizationHeaderUserHttpHandler`)
- When Azure SignalR Service is enabled in the application

**Workaround:**
- Use Manual Token Acquisition or IDownstreamApi approaches when Azure SignalR is required
- Or disable Azure SignalR if MessageHandler approach is preferred

## Architecture Summary

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   WebApp        │    │   APINoGraph     │    │   APIWithGraph  │
│ (Blazor Server) │    │ (App Roles)      │    │ (Graph + Scopes)│
│                 │    │                  │    │                 │
│ ┌─────────────┐ │    │ [Authorize(      │    │ [AuthorizeFor   │
│ │ ApiService  │ │───▶│  Roles="..."")]  │    │  Scopes=...)]   │
│ └─────────────┘ │    │                  │    │                 │
│ ┌─────────────┐ │    │ /api/hello       │    │ /api/profile/   │
│ │Downstream   │ │    │                  │    │  mobile-greeting│
│ │ApiService   │ │    │                  │    │                 │
│ └─────────────┘ │    │                  │    │ ┌─────────────┐ │
│ ┌─────────────┐ │    │                  │    │ │ Microsoft   │ │
│ │ApiService   │ │    │                  │    │ │ Graph SDK   │ │
│ │WithHandler  │ │    │                  │    │ └─────────────┘ │
│ └─────────────┘ │    │                  │    │                 │
│                 │    │                  │    │                 │
│ ┌─────────────┐ │    └──────────────────┘    └─────────────────┘
│ │ Azure       │ │           ▲                        ▲
│ │ SignalR     │ │           │                        │
│ └─────────────┘ │           │ ForApp                 │ ForUser
│ ┌─────────────┐ │           │ (Client                │ (Delegated
│ │ Redis Token │ │           │  Credentials)          │  Permissions)
│ │ Cache       │ │           │                        │
│ └─────────────┘ │           │                        │
└─────────────────┘           └────────────────────────┘
                                     Azure AD
```

## Disclaimer

This Sample Code is provided for the purpose of illustration only and is not
intended to be used in a production environment. THIS SAMPLE CODE AND ANY RELATED
INFORMATION ARE PROVIDED 'AS IS' WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED
OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF
MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
