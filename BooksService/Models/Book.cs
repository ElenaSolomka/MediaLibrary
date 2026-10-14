namespace BooksService.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Publisher { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Annotation { get; set; } = string.Empty;
    public List<int> RelatedMovieIds { get; set; } = new();
}
