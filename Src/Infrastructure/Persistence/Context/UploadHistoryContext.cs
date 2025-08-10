using Microsoft.EntityFrameworkCore;
using ReceivedFile.Model;

public class UploadHistoryContext : DbContext
{
    public UploadHistoryContext(DbContextOptions<UploadHistoryContext> options)
        : base(options)
    {
    }

    public DbSet<ReceivedFileModel> UploadHistory { get; set; }
    public DbSet<CharacterModel> Character { get; set; }
    public DbSet<FileDataModel> FileData { get; set; }
    public DbSet<EpisodeModel> Episode { get; set; }
    public DbSet<LocationModel> Location { get; set; }
    public DbSet<FileDataEpisodeModel> FileDataEpisode { get; set; }
    public DbSet<CharacterEpisodeModel> CharacterEpisode { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // chaves primárias
        modelBuilder.Entity<EpisodeModel>().HasKey(e => e.Id);
        modelBuilder.Entity<CharacterModel>().HasKey(c => c.Id);
        modelBuilder.Entity<FileDataModel>().HasKey(e => e.Id);
        modelBuilder.Entity<LocationModel>().HasKey(l => l.Id);
        modelBuilder.Entity<ReceivedFileModel>().HasKey(r => r.Id);

        // chaves estrangeiras
        modelBuilder.Entity<CharacterModel>()
            .HasOne(c => c.Origin)
            .WithMany(l => l.AsOriginFor)
            .HasForeignKey(c => c.OriginId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CharacterModel>()
            .HasOne(c => c.Location)
            .WithMany(l => l.AsLocationFor)
            .HasForeignKey(c => c.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileDataEpisodeModel>()
            .HasKey(fde => new { fde.FileDataId, fde.EpisodeId });

        modelBuilder.Entity<FileDataEpisodeModel>()
            .HasOne(fde => fde.FileData)
            .WithMany(fd => fd.FileDataEpisodes)
            .HasForeignKey(fde => fde.FileDataId);

        modelBuilder.Entity<FileDataEpisodeModel>()
            .HasOne(fde => fde.Episode)
            .WithMany(e => e.FileDataEpisodes)
            .HasForeignKey(fde => fde.EpisodeId);

        modelBuilder.Entity<CharacterEpisodeModel>()
            .HasKey(ce => new { ce.CharacterId, ce.EpisodeId });

        modelBuilder.Entity<CharacterEpisodeModel>()
            .HasOne(ce => ce.Character)
            .WithMany(c => c.CharacterEpisodes)
            .HasForeignKey(ce => ce.CharacterId);

        modelBuilder.Entity<CharacterEpisodeModel>()
            .HasOne(ce => ce.Episode)
            .WithMany(e => e.CharacterEpisodes)
            .HasForeignKey(ce => ce.EpisodeId);

        // índices
        modelBuilder.Entity<CharacterEpisodeModel>().HasIndex(ce => ce.CharacterId);
        modelBuilder.Entity<CharacterEpisodeModel>().HasIndex(ce => ce.EpisodeId);
        modelBuilder.Entity<FileDataEpisodeModel>().HasIndex(fde => fde.FileDataId);
        modelBuilder.Entity<FileDataEpisodeModel>().HasIndex(fde => fde.EpisodeId);
        modelBuilder.Entity<CharacterModel>().HasIndex(c => c.OriginId);
        modelBuilder.Entity<CharacterModel>().HasIndex(c => c.LocationId);
    }
}
