public static class EpisodeMapper
{
    public static EpisodeModel ToModel(EpisodeEntity entity)
    {
        return new EpisodeModel
        {
            Id = entity.Id,
            Name = entity.Name,
            AirDate = entity.AirDate,
            Episode = entity.Episode
        };
    }
    public static EpisodeEntity ToEntity(EpisodeModel model)
    {
        return new EpisodeEntity
        {
            Id = model.Id,
            Name = model.Name,
            AirDate = model.AirDate,
            Episode = model.Episode,
            CharacterEpisodes = model.CharacterEpisodes?
                .Select(CharacterEpisodeMapper.ToEntity)
                .ToList() ?? new()
        };
    }

}
