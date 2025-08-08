namespace UploadHistory.Repository;

using ReceivedFile.Entity;
using UploadHistory.RepositoryInterface;
using UploadHistory.Mapper;
using Microsoft.EntityFrameworkCore;


public class UploadHistoryRepository : IUploadHistoryRepository
{
    private readonly UploadHistoryContext _context;

    public UploadHistoryRepository(UploadHistoryContext context)
    {
        _context = context;
    }

    public async Task criaHistoricoArquivo(ReceivedFileEntity receivedFile)
    {
        var model = UploadHistoryMapper.ToModel(receivedFile);
        await _context.AddAsync(model);
        await _context.SaveChangesAsync();
    }
    public async Task AtualizaStatus(string processId, string novoStatus)
    {
        var historico = await _context.UploadHistory.FindAsync(processId);
        if (historico is null) return;

        historico.Status = novoStatus;
        await _context.SaveChangesAsync();
    }
    public async Task AtualizaInicioProcessamento(string processId, string novoStatus, DateTime startTime)
    {
        var historico = await _context.UploadHistory.FindAsync(processId);
        if (historico is null) return;

        historico.Status = novoStatus;
        historico.StartTime = startTime;

        await _context.SaveChangesAsync();
    }
    public async Task AtualizaFimProcessamento(string processId, string novoStatus, DateTime endTime)
    {
        var historico = await _context.UploadHistory.FindAsync(processId);
        if (historico is null) return;

        historico.Status = novoStatus;
        historico.EndTime = endTime;

        await _context.SaveChangesAsync();
    }
    public async Task<ReceivedFileEntity?> BuscarStatusArquivo(string processId)
    {
        var model = await _context.UploadHistory
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == processId);

        return model is null ? null : UploadHistoryMapper.ToEntity(model);
    }
    public async Task SalvarEpisodios(List<EpisodeEntity> episodes)
    {
        var episodeModels = episodes.Select(EpisodeMapper.ToModel).ToList();

        var existingIds = await _context.Episode
            .AsNoTracking()
            .Select(e => e.Id)
            .ToListAsync();

        var novosEpisodios = episodeModels
            .Where(e => !existingIds.Contains(e.Id))
            .ToList();

        if (novosEpisodios.Any())
        {
            await _context.Episode.AddRangeAsync(novosEpisodios);
            await _context.SaveChangesAsync();
        }
    }
    public async Task SalvarPersonagens(List<CharacterEntity> characters)
    {
        var characterModels = characters.Select(CharacterMapper.ToModel).ToList();

        var existingIds = await _context.Character
            .AsNoTracking()
            .Select(c => c.Id)
            .ToListAsync();

        var novos = characterModels
            .Where(c => !existingIds.Contains(c.Id))
            .ToList();

        if (novos.Any())
        {
            await _context.Character.AddRangeAsync(novos);
            await _context.SaveChangesAsync();
        }
    }
    public async Task SalvarLocalizacoes(List<LocationEntity> locations)
    {
        var existingIds = await _context.Location
            .AsNoTracking()
            .Select(l => l.Id)
            .ToListAsync();

        var novasLocalizacoes = locations
            .Where(l => !existingIds.Contains(l.Id))
            .Select(LocationMapper.ToModel)
            .ToList();

        if (novasLocalizacoes.Any())
        {
            _context.Location.AddRange(novasLocalizacoes);
            await _context.SaveChangesAsync();
        }
    }
    public async Task SalvarResumoArquivo(FileDataEntity fileData)
    {
        var model = FileDataMapper.ToModel(fileData);
        await _context.FileData.AddAsync(model);
        await _context.SaveChangesAsync();

    }
    public async Task SalvarRelacaoPersonagemEpisodio(List<CharacterEpisodeEntity> entities)
    {
        var models = entities.Select(CharacterEpisodeMapper.ToModel).ToList();

        var existentes = await _context.CharacterEpisode
            .AsNoTracking()
            .Select(r => new { r.CharacterId, r.EpisodeId })
            .ToListAsync();

        var novos = models
            .Where(m => !existentes.Any(e =>
                e.CharacterId == m.CharacterId &&
                e.EpisodeId == m.EpisodeId))
            .ToList();

        if (novos.Any())
        {
            await _context.CharacterEpisode.AddRangeAsync(novos);
            await _context.SaveChangesAsync();
        }
    }
    public async Task SalvarRelacaoArquivoEpisodio(List<FileDataEpisodeEntity> relacoes)
    {
        var models = relacoes
            .Select(FileDataEpisodeMapper.ToModel)
            .ToList();

        var existentes = await _context.FileDataEpisode
            .AsNoTracking()
            .Select(fd => new { fd.FileDataId, fd.EpisodeId })
            .ToListAsync();

        var novos = models
            .DistinctBy(m => new { m.FileDataId, m.EpisodeId })
            .Where(m => !existentes.Any(e =>
                e.FileDataId == m.FileDataId &&
                e.EpisodeId == m.EpisodeId))
            .ToList();

        if (novos.Any())
        {
            await _context.FileDataEpisode.AddRangeAsync(novos);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<FileDataEntity?> ObterArquivoCompleto(string processId, int pageNumber, int pageSize, string? searchTerm, Boolean? orderByName)
    {
    var query = _context.FileData
        .Include(fd => fd.FileDataEpisodes)
            .ThenInclude(fde => fde.Episode)
                .ThenInclude(ep => ep.CharacterEpisodes)
                    .ThenInclude(ce => ce.Character)
        .ThenInclude(c => c.Origin)
        .Include(fd => fd.FileDataEpisodes)
            .ThenInclude(fde => fde.Episode)
                .ThenInclude(ep => ep.CharacterEpisodes)
                    .ThenInclude(ce => ce.Character)
        .ThenInclude(c => c.Location)
        .Where(fd => fd.Id == processId);

        var fileData = await query.FirstOrDefaultAsync();

        if (fileData == null)
            return null;

        var episodes = fileData.FileDataEpisodes
            .Select(fde => fde.Episode)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            episodes = episodes
                .Where(ep => ep.Name.Contains(searchTerm));
        }

        if (orderByName == true)
            episodes = episodes.OrderByDescending(ep => ep.Name);
        else
            episodes = episodes.OrderBy(ep => ep.Name);

        var pagedEpisodes = episodes
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        fileData.FileDataEpisodes = fileData.FileDataEpisodes
            .Where(fde => pagedEpisodes.Any(pe => pe.Id == fde.EpisodeId))
            .ToList();

        fileData.Episodes = pagedEpisodes;

        return FileDataMapper.ToEntity(fileData);
    }

}