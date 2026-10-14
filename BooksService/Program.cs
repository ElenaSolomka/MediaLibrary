using Microsoft.EntityFrameworkCore;
using BooksService.Data;
using BooksService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BooksDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("BooksDatabase")));

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
    var db = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors();

app.MapGet("/api/books", async (BooksDbContext db) =>
{
    return await db.Books.ToListAsync();
});

app.MapGet("/api/books/{id}", async (int id, BooksDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    return book is not null ? Results.Ok(book) : Results.NotFound();
});

app.MapGet("/api/books/by-movie/{movieId}", async (int movieId, BooksDbContext db) =>
{
    var books = await db.Books.ToListAsync();
    var filteredBooks = books.Where(b => b.RelatedMovieIds.Contains(movieId)).ToList();
    return Results.Ok(filteredBooks);
});

app.MapPost("/api/books", async (Book book, BooksDbContext db) =>
{
    db.Books.Add(book);
    await db.SaveChangesAsync();
    return Results.Created($"/api/books/{book.Id}", book);
});

app.MapPut("/api/books/{id}", async (int id, Book inputBook, BooksDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    book.Title = inputBook.Title;
    book.Year = inputBook.Year;
    book.Publisher = inputBook.Publisher;
    book.PageCount = inputBook.PageCount;
    book.Annotation = inputBook.Annotation;
    book.RelatedMovieIds = inputBook.RelatedMovieIds;

    await db.SaveChangesAsync();
    return Results.Ok(book);
});

app.MapDelete("/api/books/{id}", async (int id, BooksDbContext db) =>
{
    var book = await db.Books.FindAsync(id);
    if (book is null) return Results.NotFound();

    db.Books.Remove(book);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
