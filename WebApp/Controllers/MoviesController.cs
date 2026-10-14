using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class MoviesController : Controller
{
    private readonly MoviesServiceClient _moviesClient;
    private readonly BooksServiceClient _booksClient;

    public MoviesController(MoviesServiceClient moviesClient, BooksServiceClient booksClient)
    {
        _moviesClient = moviesClient;
        _booksClient = booksClient;
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _moviesClient.GetMovieByIdAsync(id);
        if (movie == null) return NotFound();

        var relatedBooks = new List<WebApp.Models.Book>();
        foreach (var bookId in movie.RelatedBookIds)
        {
            var book = await _booksClient.GetBookByIdAsync(bookId);
            if (book != null) relatedBooks.Add(book);
        }

        ViewBag.Movie = movie;
        ViewBag.RelatedBooks = relatedBooks;

        return View();
    }
}
