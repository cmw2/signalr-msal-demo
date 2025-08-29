using WebApp.Components;
using WebApp.Services;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    // options.InstanceName = "cmw2-redis:"; // Optional, for key prefixing
});
//builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration.GetConnectionString("SignalR"));

// Add services to the container.
builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddDownstreamApi("APINoGraph", builder.Configuration.GetSection("DownstreamApis:APINoGraph"))
    .AddDownstreamApi("APIWithGraph", builder.Configuration.GetSection("DownstreamApis:APIWithGraph"))
    .AddDistributedTokenCaches();

builder.Services.AddTransient<AuthorizationHeaderAppHttpHandler>();
builder.Services.AddTransient<AuthorizationHeaderUserHttpHandler>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient("APINoGraphHandler", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["APIEndpoints:APINoGraphBaseUrl"] ?? "https://localhost:7067/");
}).AddHttpMessageHandler<AuthorizationHeaderAppHttpHandler>();

builder.Services.AddHttpClient("APIWithGraphHandler", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["APIEndpoints:APIWithGraphBaseUrl"] ?? "https://localhost:7087/");
}).AddHttpMessageHandler<AuthorizationHeaderUserHttpHandler>();

builder.Services.AddScoped<ApiService>();
builder.Services.AddScoped<DownstreamApiService>();
builder.Services.AddScoped<ApiServiceWithHandler>();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();

app.UseStaticFiles();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();
