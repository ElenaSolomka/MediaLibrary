using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services;

public class BooksServiceClient
{
    private readonly HttpClient _httpClient;

    public BooksServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["ServiceUrls:BooksService"];
        _httpClient.BaseAddress = new Uri(baseUrl!);
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Book>>("/api/books") ?? new();
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Book>($"/api/books/{id}");
    }

    public async Task<List<Book>> GetBooksByMovieIdAsync(int movieId)
    {
        return await _httpClient.GetFromJsonAsync<List<Book>>($"/api/books/by-movie/{movieId}") ?? new();
    }

    public async Task<HttpResponseMessage> CreateBookAsync(Book book)
    {
        return await _httpClient.PostAsJsonAsync("/api/books", book);
    }

    public async Task<HttpResponseMessage> UpdateBookAsync(int id, Book book)
    {
        return await _httpClient.PutAsJsonAsync($"/api/books/{id}", book);
    }

    public async Task<HttpResponseMessage> DeleteBookAsync(int id)
    {
        return await _httpClient.DeleteAsync($"/api/books/{id}");
    }
}
