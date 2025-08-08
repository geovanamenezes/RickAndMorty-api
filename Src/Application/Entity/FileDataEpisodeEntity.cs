public class FileDataEpisodeEntity
{
    public string FileDataId { get; set; } = null!;
    public int EpisodeId { get; set; }
    public EpisodeEntity? Episode { get; set; }
    public FileDataEntity? FileData { get; set; } 

}