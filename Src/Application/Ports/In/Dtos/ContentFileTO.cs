public class ContentFileTO
{
    public int EpisodeId { get; set; }
    public int CharacterId { get; set; }
    public string CharacterName { get; set; } = string.Empty;
    public int LocationId { get; set; }

    public bool IsValid()
    {
        return EpisodeId > 0
            && CharacterId > 0
            && !string.IsNullOrWhiteSpace(CharacterName)
            && LocationId > 0;
    }
}
