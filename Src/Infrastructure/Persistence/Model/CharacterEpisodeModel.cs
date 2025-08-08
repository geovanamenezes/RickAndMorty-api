public class CharacterEpisodeModel
{
    public int CharacterId { get; set; }
    public CharacterModel Character { get; set; } = null!;
    public int EpisodeId { get; set; }
    public EpisodeModel Episode { get; set; } = null!;
}