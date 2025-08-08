public static class FileDataEpisodeMapper
{
    public static FileDataEpisodeModel ToModel(FileDataEpisodeEntity entity)
    {
        return new FileDataEpisodeModel
        {
            FileDataId = entity.FileDataId,
            EpisodeId = entity.EpisodeId
        };
    }

    public static FileDataEpisodeEntity ToEntity(FileDataEpisodeModel model)
    {
        return new FileDataEpisodeEntity
        {
            FileDataId = model.FileDataId,
            EpisodeId = model.EpisodeId
        };
    }
}
