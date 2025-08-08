public class EpisodeTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Episode { get; set; } = string.Empty;
    public string AirDate { get; set; } = string.Empty;
    public List<CharacterTO> Characters { get; set; } = new();
}
