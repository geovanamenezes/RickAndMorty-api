public class CharacterEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int? OriginId { get; set; }
    public int? LocationId { get; set; }
    public LocationEntity? Origin { get; set; }
    public LocationEntity? Location { get; set; }
    public List<CharacterEpisodeEntity> CharacterEpisodes { get; set; } = new();

}
