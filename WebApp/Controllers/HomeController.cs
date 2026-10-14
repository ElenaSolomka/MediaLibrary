using Microsoft.AspNetCore.Mvc;
using WebApp.Services;

namespace WebApp.Controllers;

public class HomeController : Controller
{
    private readonly MoviesServiceClient _moviesClient;
    private readonly BooksServiceClient _booksClient;

    public HomeController(MoviesServiceClient moviesClient, BooksServiceClient booksClient)
    {
        _moviesClient = moviesClient;
        _booksClient = booksClient;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _moviesClient.GetAllMoviesAsync();
        var books = await _booksClient.GetAllBooksAsync();

        ViewBag.Movies = movies;
        ViewBag.Books = books;

        return View();
    }
}
