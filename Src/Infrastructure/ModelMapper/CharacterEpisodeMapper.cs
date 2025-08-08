public static class CharacterEpisodeMapper
{
    public static CharacterEpisodeModel ToModel(CharacterEpisodeEntity entity)
    {
        return new CharacterEpisodeModel
        {
            CharacterId = entity.CharacterId,
            EpisodeId = entity.EpisodeId,
        };
    }
    public static CharacterEpisodeEntity ToEntity(CharacterEpisodeModel model)
    {
        return new CharacterEpisodeEntity
        {
            CharacterId = model.CharacterId,
            EpisodeId = model.EpisodeId,
            Character = model.Character != null
                ? new CharacterEntity
                {
                    Id = model.Character.Id,
                    Name = model.Character.Name,
                    Status = model.Character.Status,
                    Species = model.Character.Species,
                    Type = model.Character.Type,
                    Gender = model.Character.Gender,
                    OriginId = model.Character.OriginId,
                    LocationId = model.Character.LocationId,
                    Origin = model.Character.Origin != null
                        ? new LocationEntity
                        {
                            Id = model.Character.Origin.Id,
                            Name = model.Character.Origin.Name,
                            Type = model.Character.Origin.Type,
                            Dimension = model.Character.Origin.Dimension,
                            Url = model.Character.Origin.Url
                        }
                        : null,
                    Location = model.Character.Location != null
                        ? new LocationEntity
                        {
                            Id = model.Character.Location.Id,
                            Name = model.Character.Location.Name,
                            Type = model.Character.Location.Type,
                            Dimension = model.Character.Location.Dimension,
                            Url = model.Character.Location.Url
                        }
                        : null
                }
                : null,

        };
    }
}
