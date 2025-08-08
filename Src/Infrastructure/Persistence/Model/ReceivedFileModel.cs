namespace ReceivedFile.Model;
public class ReceivedFileModel
{
    public ReceivedFileModel() 
    {
        Id = string.Empty;
        FilePath = string.Empty;
        Status = string.Empty;
        CreatedTimestamp = DateTime.UtcNow;
    }

    public ReceivedFileModel(string id, string filePath, DateTime createdTimestamp, DateTime startTime, DateTime endTime, string status)
    {
        this.Id = id;
        this.FilePath = filePath;
        this.CreatedTimestamp = createdTimestamp;
        this.StartTime = startTime;
        this.EndTime = endTime;
        this.Status = status;
    }

    public string Id { get; set; }
    public string FilePath { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
}
