using System.Net.Http.Json;
using WebApp.Models;

namespace WebApp.Services;

public class MoviesServiceClient
{
    private readonly HttpClient _httpClient;

    public MoviesServiceClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        var baseUrl = configuration["ServiceUrls:MoviesService"];
        _httpClient.BaseAddress = new Uri(baseUrl!);
    }

    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Movie>>("/api/movies") ?? new();
    }

    public async Task<Movie?> GetMovieByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<Movie>($"/api/movies/{id}");
    }

    public async Task<List<Movie>> GetMoviesByBookIdAsync(int bookId)
    {
        return await _httpClient.GetFromJsonAsync<List<Movie>>($"/api/movies/by-book/{bookId}") ?? new();
    }

    public async Task<HttpResponseMessage> CreateMovieAsync(Movie movie)
    {
        return await _httpClient.PostAsJsonAsync("/api/movies", movie);
    }

    public async Task<HttpResponseMessage> UpdateMovieAsync(int id, Movie movie)
    {
        return await _httpClient.PutAsJsonAsync($"/api/movies/{id}", movie);
    }

    public async Task<HttpResponseMessage> DeleteMovieAsync(int id)
    {
        return await _httpClient.DeleteAsync($"/api/movies/{id}");
    }
}
