public class EpisodeModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string AirDate { get; set; } = string.Empty;
    public string Episode { get; set; } = string.Empty;
    public List<CharacterEpisodeModel> CharacterEpisodes { get; set; } = new();
    public List<FileDataEpisodeModel> FileDataEpisodes { get; set; } = null!;
}