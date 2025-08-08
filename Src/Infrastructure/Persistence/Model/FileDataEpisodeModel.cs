public class FileDataEpisodeModel
{
    public string FileDataId { get; set; } = null!;
    public FileDataModel FileData { get; set; } = null!;

    public int EpisodeId { get; set; }
    public EpisodeModel Episode { get; set; } = null!;
}