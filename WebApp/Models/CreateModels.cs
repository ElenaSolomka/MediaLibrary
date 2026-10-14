using System.ComponentModel.DataAnnotations;

namespace WebApp.Models;

public class MovieCreateModel
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    public string Director { get; set; } = string.Empty;
    public string ProductionCompany { get; set; } = string.Empty;
    public string MainActors { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? RelatedBookIdsString { get; set; }
    
    public Movie ToMovie()
    {
        var movie = new Movie
        {
            Title = Title,
            Year = Year,
            Director = Director,
            ProductionCompany = ProductionCompany,
            MainActors = MainActors,
            Description = Description,
            ImageUrl = ImageUrl,
            RelatedBookIds = new List<int>()
        };
        
        if (!string.IsNullOrWhiteSpace(RelatedBookIdsString))
        {
            movie.RelatedBookIds = RelatedBookIdsString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
        }
        
        return movie;
    }
}

public class BookCreateModel
{
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public int Year { get; set; }
    
    public string Publisher { get; set; } = string.Empty;
    public int PageCount { get; set; }
    public string Annotation { get; set; } = string.Empty;
    public string? RelatedMovieIdsString { get; set; }
    
    public Book ToBook()
    {
        var book = new Book
        {
            Title = Title,
            Year = Year,
            Publisher = Publisher,
            PageCount = PageCount,
            Annotation = Annotation,
            RelatedMovieIds = new List<int>()
        };
        
        if (!string.IsNullOrWhiteSpace(RelatedMovieIdsString))
        {
            book.RelatedMovieIds = RelatedMovieIdsString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => int.TryParse(s.Trim(), out var id) ? id : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .ToList();
        }
        
        return book;
    }
}
