using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.Queries;
using Sportradar.Calendar.Infrastructure.InMemory;
using Sportradar.Calendar.Infrastructure.Persistence;
using Sportradar.Calendar.Presentation.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GetUpcomingEvents>();
builder.Services.AddSingleton<IEntityEventRepository, InMemoryEventRepository>();

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
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

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();