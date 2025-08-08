namespace UploadHistory.Mapper;

using ReceivedFile.Entity;
using ReceivedFile.Model;
public static class UploadHistoryMapper
{
    public static ReceivedFileModel ToModel(ReceivedFileEntity entity)
    {
        return new ReceivedFileModel
        {
            Id = entity.Id,
            FilePath = entity.FilePath,
            CreatedTimestamp = entity.CreatedTimestamp,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            Status = entity.Status
        };
    }
    public static ReceivedFileEntity ToEntity(ReceivedFileModel model)
    {
        return new ReceivedFileEntity(
            model.Id,
            model.FilePath,
            model.CreatedTimestamp,
            model.StartTime,
            model.EndTime,
            model.Status
        );
    }

}