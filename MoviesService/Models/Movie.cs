namespace MoviesService.Models;

public class Movie
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Director { get; set; } = string.Empty;
    public string ProductionCompany { get; set; } = string.Empty;
    public string MainActors { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public List<int> RelatedBookIds { get; set; } = new();
}
