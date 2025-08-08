using UploadHistory.RepositoryInterface;
using CompleteFile.UsecaseInterface;
using RequestRickAndMorty.Ports;
using RickAndMortyApi.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CompleteFile.Usecase;

public class CompleteFileUseCase : ICompleteFile
{
    private readonly IUploadHistoryRepository _uploadHistoryRepository;

    public CompleteFileUseCase(IUploadHistoryRepository repository)
    {
        _uploadHistoryRepository = repository;
    }

    public async Task<FileDataEntity> CompletaArquivo(List<ContentFileEntity> contentsFile, string processId)
    {
        try
        {
            await _uploadHistoryRepository.AtualizaInicioProcessamento(processId,"EM PROCESSAMENTO", DateTime.UtcNow);
            var arquivoCompleto = new FileDataEntity
            {
                Id = processId,
                FileDataEpisodes = new List<FileDataEpisodeEntity>()
            };

            var episodeDict = new Dictionary<int, EpisodeEntity>();
            var characterDict = new Dictionary<int, CharacterEntity>();
            var locationDict = new Dictionary<int, LocationEntity>();

            foreach (var content in contentsFile)
            {
                if (!episodeDict.TryGetValue(content.EpisodeId, out var episode))
                {
                    Console.WriteLine($"buscando episódio: {content.EpisodeId}");
                    episode = await BuscarEpisodioComPersonagens(content.EpisodeId, characterDict, locationDict);
                    episodeDict[content.EpisodeId] = episode;
                }

                arquivoCompleto.FileDataEpisodes.Add(new FileDataEpisodeEntity
                {
                    Episode = episode,
                    EpisodeId = episode.Id,
                    FileData = arquivoCompleto,
                    FileDataId = arquivoCompleto.Id
                });
            }

            PreencherTotais(arquivoCompleto, characterDict.Values.ToList());

            await _uploadHistoryRepository.SalvarLocalizacoes(locationDict.Values.ToList());
            await _uploadHistoryRepository.SalvarPersonagens(characterDict.Values.ToList());
            await _uploadHistoryRepository.SalvarEpisodios(episodeDict.Values.ToList());
            await _uploadHistoryRepository.SalvarResumoArquivo(arquivoCompleto);

            var characterEpisodeRelations = episodeDict.Values
                .SelectMany(ep => ep.CharacterEpisodes ?? Enumerable.Empty<CharacterEpisodeEntity>())
                .ToList();

            var fileDataEpisodeRelations = arquivoCompleto.FileDataEpisodes.ToList();

            await _uploadHistoryRepository.SalvarRelacaoPersonagemEpisodio(characterEpisodeRelations);
            await _uploadHistoryRepository.SalvarRelacaoArquivoEpisodio(fileDataEpisodeRelations);
            await _uploadHistoryRepository.AtualizaFimProcessamento(processId,"CONCLUÍDO COM SUCESSO", DateTime.UtcNow);

            return arquivoCompleto;
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("Erro ao salvar na base:");
            Console.WriteLine(ex.InnerException?.Message);
            await _uploadHistoryRepository.AtualizaFimProcessamento(processId,"ERRO INTERNO DURANTE O PROCESSAMENTO", DateTime.UtcNow);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro no processamento do arquivo: ");
            Console.WriteLine(ex.InnerException?.Message);
            await _uploadHistoryRepository.AtualizaFimProcessamento(processId,"ERRO INTERNO DURANTE O PROCESSAMENTO", DateTime.UtcNow);
            throw;
        }
    }

    private async Task<EpisodeEntity> BuscarEpisodioComPersonagens(
        int episodeId,
        Dictionary<int, CharacterEntity> characterDict,
        Dictionary<int, LocationEntity> locationDict)
    {
        var episodeTo = await RequestRickAndMortyApi.BuscarEpisodio(episodeId);
        if (episodeTo == null)
            throw new InvalidDataException($"Falha ao obter episódio {episodeId}");

        var episode = new EpisodeEntity
        {
            Id = episodeTo.Id,
            Name = episodeTo.Name,
            AirDate = episodeTo.AirDate,
            Episode = episodeTo.Episode,
            CharacterEpisodes = new List<CharacterEpisodeEntity>()
        };

        var characterIds = episodeTo.Characters
            .Select(url => int.Parse(url.Split('/').Last()))
            .ToList();

        foreach (var characterId in characterIds)
        {
            Console.WriteLine($"buscando personagem: {characterId}");

            if (!characterDict.TryGetValue(characterId, out var character))
            {
                character = await BuscarPersonagemComLocalizacoes(characterId);

                if (character.Origin is not null)
                    locationDict[character.Origin.Id] = character.Origin;

                if (character.Location is not null)
                    locationDict[character.Location.Id] = character.Location;

                characterDict[character.Id] = character;
            }

            episode.CharacterEpisodes.Add(new CharacterEpisodeEntity
            {
                Character = character,
                CharacterId = character.Id,
                Episode = episode,
                EpisodeId = episode.Id
            });
        }

        return episode;
    }

    private async Task<CharacterEntity> BuscarPersonagemComLocalizacoes(int characterId)
    {
        var characterTo = await RequestRickAndMortyApi.BuscarPersonagem(characterId);
        if (characterTo == null)
            throw new InvalidDataException($"Falha ao obter personagem {characterId}");

        LocationTo? origin = null;
        if (!string.IsNullOrEmpty(characterTo.Origin?.Url))
            origin = await RequestRickAndMortyApi.BuscarLocalizacaoPorUrl(characterTo.Origin.Url);

        LocationTo? location = null;
        if (!string.IsNullOrEmpty(characterTo.Location?.Url))
            location = await RequestRickAndMortyApi.BuscarLocalizacaoPorUrl(characterTo.Location.Url);

        return new CharacterEntity
        {
            Id = characterTo.Id,
            Name = characterTo.Name,
            Gender = characterTo.Gender,
            Origin = origin != null ? MapearLocation(origin) : null,
            Location = location != null ? MapearLocation(location) : null,
            Status = characterTo.Status,
            Species = characterTo.Species,
            Type = characterTo.Type,
        };
    }

    private LocationEntity MapearLocation(LocationTo locationTo)
    {
        return new LocationEntity
        {
            Id = locationTo.Id,
            Name = locationTo.Name,
            Type = locationTo.Type,
            Dimension = locationTo.Dimension,
            Url = locationTo.Url
        };
    }

    private void PreencherTotais(FileDataEntity arquivo, List<CharacterEntity> allCharacters)
    {
        Console.WriteLine("preenchendo totais");

        arquivo.TotalLocations = allCharacters
            .Select(c => c.Location?.Id)
            .Where(id => id.HasValue)
            .Distinct()
            .Count();

        arquivo.TotalFemaleCharacters = allCharacters.Count(c => c.Gender == "Female");
        arquivo.TotalMaleCharacters = allCharacters.Count(c => c.Gender == "Male");
        arquivo.TotalGenderlessCharacters = allCharacters.Count(c => c.Gender == "Genderless");
        arquivo.TotalGenderUnknownCharacters = allCharacters.Count(c => c.Gender == "unknown");
    }
}
