using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;
using Sportradar.Calendar.Application.Queries;
using Sportradar.Calendar.Infrastructure.InMemory;
using Sportradar.Calendar.Infrastructure.Persistence;
using Sportradar.Calendar.Presentation.Web.Components;
using Sportradar.Calendar.Presentation.Web.Requests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<GetUpcomingEvents>();
builder.Services.AddScoped<GetAllSports>();
builder.Services.AddScoped<IEntityEventRepository, EfEntityEventRepository>();
builder.Services.AddScoped<ISportRepository, EfSportRepository>();

builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

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

var eventsApi = app.MapGroup("/api/events");

eventsApi.MapGet(
    "/",
    async Task<IResult> (
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? sportId,
        IEntityEventRepository repository,
        CancellationToken cancellationToken) =>
    {
        var rangeStart = from ?? DateTimeOffset.UtcNow.AddMonths(-1);
        var rangeEnd = to ?? DateTimeOffset.UtcNow.AddMonths(12);

        var events = await repository.GetUpcomingAsync(rangeStart, rangeEnd, sportId, cancellationToken);
        return Results.Ok(events);
    });

eventsApi.MapGet(
    "{id:int}",
    async Task<IResult> (
        int id,
        IEntityEventRepository repository,
        CancellationToken cancellationToken) =>
    {
        var entity = await repository.GetAsync(id, cancellationToken);
        return entity is null ? Results.NotFound() : Results.Ok(entity);
    });

eventsApi.MapPost(
    "/",
    async Task<IResult> (
        CreateEventRequest request,
        IEntityEventRepository repository,
        CancellationToken cancellationToken) =>
    {
        try
        {
            var validationErrors = new Dictionary<string, string[]>(StringComparer.Ordinal);

            if (request.SportId <= 0)
            {
                validationErrors[nameof(request.SportId)] = new[] { "A valid sport is required." };
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                validationErrors[nameof(request.Title)] = new[] { "Title is required." };
            }

            if (request.StartsAt == default)
            {
                validationErrors[nameof(request.StartsAt)] = new[] { "Start time is required." };
            }

            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var dto = new CreateEventDto(request.SportId, request.StartsAt, request.Title);
            var newId = await repository.AddAsync(dto, cancellationToken);

            var created = await repository.GetAsync(newId, cancellationToken);
            return Results.Created($"/api/events/{newId}", created);
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { error = ex.Message });
        }
    });

app.Run();