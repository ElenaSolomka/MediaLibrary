using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class BooksController : Controller
{
    private readonly MoviesServiceClient _moviesClient;
    private readonly BooksServiceClient _booksClient;

    public BooksController(MoviesServiceClient moviesClient, BooksServiceClient booksClient)
    {
        _moviesClient = moviesClient;
        _booksClient = booksClient;
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _booksClient.GetBookByIdAsync(id);
        if (book == null) return NotFound();

        var relatedMovies = await _moviesClient.GetMoviesByBookIdAsync(id);

        ViewBag.Book = book;
        ViewBag.RelatedMovies = relatedMovies;

        return View();
    }
}
