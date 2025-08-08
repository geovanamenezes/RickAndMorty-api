public static class CharacterMapper
{
public static CharacterModel ToModel(CharacterEntity entity)
{
    return new CharacterModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Status = entity.Status,
            Species = entity.Species,
            Type = entity.Type,
            Gender = entity.Gender,
            OriginId = entity.Origin != null ? entity.Origin.Id : null,
            LocationId = entity.Location != null ? entity.Location.Id : null
        };
}
    public static CharacterEntity ToEntity(CharacterModel model)
    {
        return new CharacterEntity
        {
            Id = model.Id,
            Name = model.Name,
            Status = model.Status,
            Species = model.Species,
            Type = model.Type,
            Gender = model.Gender,
            OriginId = model.OriginId,
            LocationId = model.LocationId,
            Origin = model.Origin != null ? LocationMapper.ToEntity(model.Origin) : null,
            Location = model.Location != null ? LocationMapper.ToEntity(model.Location) : null
        };
    }

}
