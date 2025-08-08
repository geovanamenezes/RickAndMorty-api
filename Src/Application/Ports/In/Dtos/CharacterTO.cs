public class CharacterTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Species { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public LocationTO? Origin { get; set; }
    public LocationTO? Location { get; set; }
}
