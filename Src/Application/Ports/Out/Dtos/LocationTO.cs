namespace RickAndMortyApi.DTOs;
public class LocationTo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Dimension { get; set; } = string.Empty;
    public List<string> Residents { get; set; } = new();
    public string Url { get; set; } = string.Empty;
    public DateTime Created { get; set; }
}