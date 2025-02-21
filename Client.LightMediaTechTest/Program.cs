using Client.LightMediaTechTest.Components;
using Server.LightMediaTechTest.Models;
using Server.LightMediaTechTest.Services.EventManager;
using Server.LightMediaTechTest.Services.UserManager;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor();
builder.Services.AddRazorPages();
builder.Services.AddLogging();

builder.Services.AddScoped<UserManager, UserManager>();
builder.Services.AddScoped<EventManager, EventManager>();
builder.Services.AddSingleton<AppSettings, AppSettings>();

builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(Presentation.LightMediaTechTest.Components.MainLayout.MainLayout).Assembly);

app.MapRazorPages();

app.Run();