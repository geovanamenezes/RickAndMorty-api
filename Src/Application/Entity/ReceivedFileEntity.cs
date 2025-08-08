namespace ReceivedFile.Entity;
public class ReceivedFileEntity
{
    public ReceivedFileEntity(
        string id,
        string filePath,
        DateTime createdTimestamp,
        DateTime? startTime,
        DateTime? endTime,
        string status)
    {
        Id = id;
        FilePath = filePath;
        CreatedTimestamp = createdTimestamp;
        StartTime = startTime;
        EndTime = endTime;
        Status = status;
    }
    public ReceivedFileEntity(string status, string id, DateTime CreatedTimestamp)
    {
        FilePath = "Uploads/" + id.ToString();
        Id = id;
        Status = status;
    }
    public string Id { get; set; }
    public string FilePath { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public string Status { get; set; }
}