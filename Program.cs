using ctrlAltDefeatApp.Components;
using ctrlAltDefeatApp.Data;

using MudBlazor.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ctrlAltDefeatApp.Data.DatabaseContext;
using ctrlAltDefeatApp.Data.Services;



var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("SqliteDB");

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();
builder.Services.AddServerSideBlazor();
builder.Services.AddDbContextFactory<CtrlAltDefeatDatabaseContext>(options => options.UseSqlite(connectionString));
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<DevOpsWorkItemService>();
builder.Services.AddScoped<EstimateService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<XperiencePointsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
