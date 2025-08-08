namespace UploadHistory.Usecase;
using UploadHistory.UsecaseInterface;
using UploadHistory.RepositoryInterface;
using ReceivedFile.Entity;
using Serilog;

public class UploadHistoryUseCase : IUploadHistory
{
    private readonly IUploadHistoryRepository uploadHistoryRepository;
    public UploadHistoryUseCase(IUploadHistoryRepository repository)
    {
        uploadHistoryRepository = repository;
    }

    public async Task<string> ProcessaArquivo(IFormFile file)
    {
        Log.Information($"Iniciando processamento do arquivo: {file.FileName}");
        var processId = Guid.NewGuid().ToString();

        await ValidarArquivo(file);
        Log.Information($"Arquivo {file.FileName} validado com sucesso.");

        SalvarArquivoFisico(file, processId);
        Log.Information($"Arquivo {file.FileName} salvo com sucesso no diretório.");

        await RegistrarArquivoRecebido(processId);
        Log.Information($"Arquivo {file.FileName} registrado com sucesso no base de dados.");
        return processId;
    }
    public async Task<ReceivedFileEntity?> BuscaStatusArquivo(string processId)
    {
        return await uploadHistoryRepository.BuscarStatusArquivo(processId);
    }
    private async Task<List<ContentFileTO>> ValidarArquivo(IFormFile file)
    {
        List<ContentFileTO> registros = new();

        using var reader = new StreamReader(file.OpenReadStream());
        string? line;
        int lineNumber = 0;

        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (lineNumber == 1) continue;

            var parts = line.Split(',');

            if (parts.Length < 4)
            {
                Log.Error($"Linha {lineNumber} incompleta: {line}");
                throw new InvalidDataException($"Linha {lineNumber} incompleta.");
            }

            var registro = new ContentFileTO
            {
                EpisodeId = int.TryParse(parts[0], out var ep) ? ep : 0,
                CharacterId = int.TryParse(parts[1], out var ch) ? ch : 0,
                CharacterName = parts[2],
                LocationId = int.TryParse(parts[3], out var loc) ? loc : 0
            };

            if (!registro.IsValid())
            {
                Log.Error($"Linha {lineNumber} inválida: {line}");
                throw new InvalidDataException($"Linha {lineNumber} inválida.");
            }

            registros.Add(registro);
        }

        if (!registros.Any())
        {
            Log.Error("Arquivo não possui registros válidos.");
            throw new InvalidDataException("Arquivo não possui registros válidos.");
        }

        return registros;
    }
    private string SalvarArquivoFisico(IFormFile file, String processId)
    {
        var fileName = processId;
        var uploadPath = Path.Combine("Uploads", fileName);
        Directory.CreateDirectory("Uploads");

        using var stream = new FileStream(uploadPath, FileMode.Create);
        file.CopyTo(stream);

        return fileName;
    }
    private async Task RegistrarArquivoRecebido(string processId)
    {   
        Log.Information($"Atualizando status do arquivo como RECEBIDO: {processId}");
        var uploadHistory = new ReceivedFileEntity("RECEBIDO", processId, DateTime.UtcNow);
        await uploadHistoryRepository.criaHistoricoArquivo(uploadHistory);
    }
public async Task<FileDataTO?> RetornaDadosArquivoCompleto(
    string processId, 
    int? pageNumber, 
    int? pageSize, 
    string? searchTerm, 
    string? orderByName)

{
    var (validPage, validPageSize) = ValidaParametrosGetArquivo(pageNumber, pageSize);
    var entity = await uploadHistoryRepository.ObterArquivoCompleto(processId, validPage, validPageSize);

    if (entity == null)
        return null;

    var episodesDto = entity.Episodes is null
        ? new List<EpisodeTO>()
        : entity.Episodes
            .Where(e => e != null)
            .Select(MapearEpisodeTO)
            .Where(e => e != null)
            .ToList()!;

    return new FileDataTO
    {
        Id = entity.Id,
        UploadeFilePath = entity.UploadeFilePath,
        TotalFemaleCharacters = entity.TotalFemaleCharacters,
        TotalMaleCharacters = entity.TotalMaleCharacters,
        TotalGenderlessCharacters = entity.TotalGenderlessCharacters,
        TotalGenderUnknownCharacters = entity.TotalGenderUnknownCharacters,
        TotalLocations = entity.TotalLocations,
        Episodes = episodesDto
    };
}
    private LocationTO? MapearLocationTO(LocationEntity? location)
    {
        if (location == null)
            return null;

        return new LocationTO
        {
            Id = location.Id,
            Name = location.Name,
            Type = location.Type,
            Dimension = location.Dimension
        };
    }

    private CharacterTO? MapearCharacterTO(CharacterEpisodeEntity? characterEpisode)
    {
        if (characterEpisode?.Character == null)
            return null;

        var character = characterEpisode.Character;

        return new CharacterTO
        {
            Id = character.Id,
            Name = character.Name,
            Status = character.Status,
            Species = character.Species,
            Type = character.Type,
            Gender = character.Gender,
            Origin = MapearLocationTO(character.Origin) ?? new LocationTO(),  // Se quiser garantir instância não-nula
            Location = MapearLocationTO(character.Location) ?? new LocationTO()
        };
    }

    private EpisodeTO? MapearEpisodeTO(EpisodeEntity? episode)
    {
        if (episode == null)
            return null;

        var charactersDto = (episode.CharacterEpisodes ?? Enumerable.Empty<CharacterEpisodeEntity>())
            .Select(MapearCharacterTO)
            .Where(c => c != null)
            .ToList()!; 

        return new EpisodeTO
        {
            Id = episode.Id,
            Name = episode.Name,
            AirDate = episode.AirDate,
            Episode = episode.Episode,
            Characters = charactersDto
        };
    }

    public static (int? Page, int? PageSize) ValidaParametrosGetArquivo(int? page, int? pageSize)
    {
        if (page == null || page == 0)
            page = 0;
        if (pageSize == null || pageSize == 0)
            pageSize = 0;

        bool pageValid = page.HasValue && page > 0;
        bool pageSizeValid = pageSize.HasValue && pageSize > 0;

        if (!pageValid && !pageSizeValid)
            return (null, null);

        int validPage = pageValid ? page.Value : 1;
        int validPageSize = pageSizeValid ? pageSize.Value : 10;

        return (validPage, validPageSize);
    }
}
