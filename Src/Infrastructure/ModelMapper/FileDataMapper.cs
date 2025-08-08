public static class FileDataMapper
{
public static FileDataModel ToModel(FileDataEntity entity)
{
    return new FileDataModel
    {
        Id = entity.Id,
        UploadeFilePath = entity.UploadeFilePath,
        TotalFemaleCharacters = entity.TotalFemaleCharacters,
        TotalMaleCharacters = entity.TotalMaleCharacters,
        TotalGenderlessCharacters = entity.TotalGenderlessCharacters,
        TotalGenderUnknownCharacters = entity.TotalGenderUnknownCharacters,
        TotalLocations = entity.TotalLocations
    };
}

    public static FileDataEntity ToEntity(FileDataModel model)
    {
        var fileDataEntity = new FileDataEntity
        {
            Id = model.Id,
            UploadeFilePath = model.UploadeFilePath,
            TotalFemaleCharacters = model.TotalFemaleCharacters,
            TotalMaleCharacters = model.TotalMaleCharacters,
            TotalGenderlessCharacters = model.TotalGenderlessCharacters,
            TotalGenderUnknownCharacters = model.TotalGenderUnknownCharacters,
            TotalLocations = model.TotalLocations,
            FileDataEpisodes = model.FileDataEpisodes?
                .Select(FileDataEpisodeMapper.ToEntity)
                .ToList() ?? new(),
            Episodes = model.Episodes?
                .Select(EpisodeMapper.ToEntity)
                .ToList() ?? new()
        };
        return fileDataEntity;
    }


}
