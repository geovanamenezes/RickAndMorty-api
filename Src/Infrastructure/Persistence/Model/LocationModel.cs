public class LocationModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Dimension { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public List<CharacterModel> AsOriginFor { get; set; } = new();
    public List<CharacterModel> AsLocationFor { get; set; } = new();
}