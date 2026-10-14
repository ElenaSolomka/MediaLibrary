using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly MoviesServiceClient _moviesClient;
    private readonly BooksServiceClient _booksClient;
    private readonly UsersDbContext _db;

    public AdminController(MoviesServiceClient moviesClient, BooksServiceClient booksClient, UsersDbContext db)
    {
        _moviesClient = moviesClient;
        _booksClient = booksClient;
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    #region Movies

    public async Task<IActionResult> Movies()
    {
        var movies = await _moviesClient.GetAllMoviesAsync();
        return View(movies);
    }

    public IActionResult CreateMovie()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMovie(MovieCreateModel model)
    {
        var movie = model.ToMovie();
        await _moviesClient.CreateMovieAsync(movie);
        return RedirectToAction(nameof(Movies));
    }

    public async Task<IActionResult> EditMovie(int id)
    {
        var movie = await _moviesClient.GetMovieByIdAsync(id);
        if (movie == null) return NotFound();
        
        var model = new MovieCreateModel
        {
            Title = movie.Title,
            Year = movie.Year,
            Director = movie.Director,
            ProductionCompany = movie.ProductionCompany,
            MainActors = movie.MainActors,
            Description = movie.Description,
            ImageUrl = movie.ImageUrl,
            RelatedBookIdsString = string.Join(",", movie.RelatedBookIds)
        };
        
        ViewBag.MovieId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMovie(int id, MovieCreateModel model)
    {
        var movie = model.ToMovie();
        movie.Id = id;
        await _moviesClient.UpdateMovieAsync(id, movie);
        return RedirectToAction(nameof(Movies));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        await _moviesClient.DeleteMovieAsync(id);
        return RedirectToAction(nameof(Movies));
    }

    #endregion

    #region Books

    public async Task<IActionResult> Books()
    {
        var books = await _booksClient.GetAllBooksAsync();
        return View(books);
    }

    public IActionResult CreateBook()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBook(BookCreateModel model)
    {
        var book = model.ToBook();
        await _booksClient.CreateBookAsync(book);
        return RedirectToAction(nameof(Books));
    }

    public async Task<IActionResult> EditBook(int id)
    {
        var book = await _booksClient.GetBookByIdAsync(id);
        if (book == null) return NotFound();
        
        var model = new BookCreateModel
        {
            Title = book.Title,
            Year = book.Year,
            Publisher = book.Publisher,
            PageCount = book.PageCount,
            Annotation = book.Annotation,
            RelatedMovieIdsString = string.Join(",", book.RelatedMovieIds)
        };
        
        ViewBag.BookId = id;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBook(int id, BookCreateModel model)
    {
        var book = model.ToBook();
        book.Id = id;
        await _booksClient.UpdateBookAsync(id, book);
        return RedirectToAction(nameof(Books));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBook(int id)
    {
        await _booksClient.DeleteBookAsync(id);
        return RedirectToAction(nameof(Books));
    }

    #endregion

    #region Users

    public async Task<IActionResult> Users()
    {
        var users = await _db.Users.ToListAsync();
        return View(users);
    }

    public IActionResult CreateUser()
    {
        ViewBag.Roles = Enum.GetValues<UserRole>();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(string login, string password, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Заполните все поля";
            ViewBag.Roles = Enum.GetValues<UserRole>();
            return View();
        }

        if (await _db.Users.AnyAsync(u => u.Login == login))
        {
            ViewBag.Error = "Пользователь с таким логином уже существует";
            ViewBag.Roles = Enum.GetValues<UserRole>();
            return View();
        }

        var user = new User
        {
            Login = login,
            Role = role
        };
        var passwordHasher = new PasswordHasher<User>();
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var currentUserId = User.FindFirst("UserId")?.Value;
        if (currentUserId == id.ToString())
        {
            return RedirectToAction(nameof(Users));
        }

        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Users));
    }

    #endregion
}
