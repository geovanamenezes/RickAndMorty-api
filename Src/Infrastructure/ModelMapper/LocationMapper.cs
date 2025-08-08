public static class LocationMapper
{
    public static LocationModel ToModel(LocationEntity entity)
    {
        return new LocationModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            Dimension = entity.Dimension,
            Url = entity.Url,
        };
    }

    public static LocationEntity ToEntity(LocationModel model)
    {
        return new LocationEntity
        {
            Id = model.Id,
            Name = model.Name,
            Type = model.Type,
            Dimension = model.Dimension,
            Url = model.Url
        };
    }
}
