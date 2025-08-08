namespace RickAndMortyApi.DTOs;
public class EpisodeTo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AirDate { get; set; } = string.Empty;
    public string Episode { get; set; } = string.Empty;
    public List<string> Characters { get; set; } = new();
    public string Url { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}