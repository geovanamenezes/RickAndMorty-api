public class EpisodeEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AirDate { get; set; } = string.Empty;
    public string Episode { get; set; } = string.Empty;
    public string? FileDataEntityId { get; set; }
    public List<CharacterEpisodeEntity> CharacterEpisodes { get; set; } = new();
    public List<FileDataEpisodeEntity> FileDataEpisodes { get; set; } = new();

}