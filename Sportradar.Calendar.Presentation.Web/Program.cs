using Microsoft.EntityFrameworkCore;
using Sportradar.Calendar.Application.Abstractions;
using Sportradar.Calendar.Application.DTOs;
using Sportradar.Calendar.Application.Queries;
using Sportradar.Calendar.Infrastructure.InMemory;
using Sportradar.Calendar.Infrastructure.Persistence;
using Sportradar.Calendar.Presentation.Web.Components;
using Sportradar.Calendar.Presentation.Web.Requests;

// I bootstrap the web app here so later I can register all the services.
var builder = WebApplication.CreateBuilder(args);

// These scoped services are the small helpers that Razor components will ask for.
builder.Services.AddScoped<GetUpcomingEvents>();
builder.Services.AddScoped<GetAllSports>();
builder.Services.AddScoped<IEntityEventRepository, EfEntityEventRepository>();
builder.Services.AddScoped<ISportRepository, EfSportRepository>();

// DbContext needs to be configured once, otherwise migrations cannot run later.
builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
{
    optionsBuilder.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Add services to the container. (Made by rider, I think)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// I force a scope here so migrations run at startup instead of first request.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline. (Made by rider, I think)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts. (Made by rider, I think)
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// group for all events API endpoints so url becomes /api/events/...
var eventsApi = app.MapGroup("/api/events");

// GET /api/events -> returns list filtered by optional range and sport
eventsApi.MapGet(
    "/",
    async Task<IResult> (
        DateTimeOffset? from,
        DateTimeOffset? to,
        int? sportId,
        IEntityEventRepository repository,
        CancellationToken cancellationToken) =>
    {
        // Default range makes sure the calendar is not empty when user hits the page.
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
        // repository handles null check, we just forward response shape
        var entity = await repository.GetAsync(id, cancellationToken);
        return entity is null ? Results.NotFound() : Results.Ok(entity);
    });

// POST /api/events -> validate body and create new event row
eventsApi.MapPost(
    "/",
    async Task<IResult> (
        CreateEventRequest request,
        IEntityEventRepository repository,
        CancellationToken cancellationToken) =>
    {
        try
        {
            // SportId is basically foreign key so zero makes no sense.
            var validationErrors = new Dictionary<string, string[]>(StringComparer.Ordinal);

            if (request.SportId <= 0)
            {
                validationErrors[nameof(request.SportId)] = new[] { "A valid sport is required." };
            }

            // Title is what we show in UI so it must have at least some letters.
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                validationErrors[nameof(request.Title)] = new[] { "Title is required." };
            }

            // Default means the struct stayed at zero time, we treat that as missing data.
            if (request.StartsAt == default)
            {
                validationErrors[nameof(request.StartsAt)] = new[] { "Start time is required." };
            }

            // If any validation happened we stop now to avoid half baked events.
            if (validationErrors.Count > 0)
            {
                return Results.ValidationProblem(validationErrors);
            }

            var dto = new CreateEventDto(request.SportId, request.StartsAt, request.Title);
            var newId = await repository.AddAsync(dto, cancellationToken);

            // load created entity so response includes full dto with sport name
            var created = await repository.GetAsync(newId, cancellationToken);
            return Results.Created($"/api/events/{newId}", created);
        }
        catch (ArgumentException ex)
        {
            // repo throws when sport is unknown, we convert to 400 with message
            return Results.BadRequest(new { error = ex.Message });
        }
    });

app.Run();