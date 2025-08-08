public class FileDataModel
{
    public List<EpisodeModel> Episodes { get; set; } = new();
    public int TotalLocations { get; set; }
    public int TotalFemaleCharacters { get; set; }
    public int TotalMaleCharacters { get; set; }
    public int TotalGenderlessCharacters { get; set; }
    public int TotalGenderUnknownCharacters { get; set; }
    public string UploadeFilePath { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public List<FileDataEpisodeModel> FileDataEpisodes { get; set; } = new();

}