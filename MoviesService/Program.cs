using Microsoft.EntityFrameworkCore;
using MoviesService.Data;
using MoviesService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MoviesDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("MoviesDatabase")));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<MoviesDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

app.MapGet("/api/movies", async (MoviesDbContext db) =>
{
    return await db.Movies.ToListAsync();
});

app.MapGet("/api/movies/{id}", async (int id, MoviesDbContext db) =>
{
    var movie = await db.Movies.FindAsync(id);
    return movie is not null ? Results.Ok(movie) : Results.NotFound();
});

app.MapGet("/api/movies/by-book/{bookId}", async (int bookId, MoviesDbContext db) =>
{
    var movies = await db.Movies.ToListAsync();
    var filteredMovies = movies.Where(m => m.RelatedBookIds.Contains(bookId)).ToList();
    return Results.Ok(filteredMovies);
});

app.MapPost("/api/movies", async (Movie movie, MoviesDbContext db) =>
{
    db.Movies.Add(movie);
    await db.SaveChangesAsync();
    return Results.Created($"/api/movies/{movie.Id}", movie);
});

app.MapPut("/api/movies/{id}", async (int id, Movie inputMovie, MoviesDbContext db) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();

    movie.Title = inputMovie.Title;
    movie.Year = inputMovie.Year;
    movie.Director = inputMovie.Director;
    movie.ProductionCompany = inputMovie.ProductionCompany;
    movie.MainActors = inputMovie.MainActors;
    movie.Description = inputMovie.Description;
    movie.ImageUrl = inputMovie.ImageUrl;
    movie.RelatedBookIds = inputMovie.RelatedBookIds;

    await db.SaveChangesAsync();
    return Results.Ok(movie);
});

app.MapDelete("/api/movies/{id}", async (int id, MoviesDbContext db) =>
{
    var movie = await db.Movies.FindAsync(id);
    if (movie is null) return Results.NotFound();

    db.Movies.Remove(movie);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
