public class CharacterModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int? OriginId { get; set; }
    public int? LocationId { get; set; }
    public LocationModel? Origin { get; set; }
    public LocationModel? Location { get; set; }
    public List<CharacterEpisodeModel> CharacterEpisodes { get; set; } = new();
}
