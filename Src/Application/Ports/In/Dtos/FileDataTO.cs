public class FileDataTO
{
    public string Id { get; set; } = string.Empty;
    public string UploadeFilePath { get; set; } = string.Empty;
    public int TotalLocations { get; set; }
    public int TotalFemaleCharacters { get; set; }
    public int TotalMaleCharacters { get; set; }
    public int TotalGenderlessCharacters { get; set; }
    public int TotalGenderUnknownCharacters { get; set; }

    public List<EpisodeTO> Episodes { get; set; } = new();
}
