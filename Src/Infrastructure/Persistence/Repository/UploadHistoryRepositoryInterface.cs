namespace UploadHistory.RepositoryInterface;
using ReceivedFile.Entity;
public interface IUploadHistoryRepository
{
    Task criaHistoricoArquivo(ReceivedFileEntity receivedFile);
    Task AtualizaStatus(string processId, string novoStatus);
    Task AtualizaInicioProcessamento(string processId, string novoStatus, DateTime startTime);
    Task AtualizaFimProcessamento(string processId, string novoStatus, DateTime endTime);
    Task<ReceivedFileEntity?> BuscarStatusArquivo(string processId);
    Task SalvarEpisodios(List<EpisodeEntity> episodes);
    Task SalvarPersonagens(List<CharacterEntity> characters);
    Task SalvarLocalizacoes(List<LocationEntity> locations);
    Task SalvarResumoArquivo(FileDataEntity fileData);
    Task SalvarRelacaoPersonagemEpisodio(List<CharacterEpisodeEntity> characterEpisodes);
    Task SalvarRelacaoArquivoEpisodio(List<FileDataEpisodeEntity> fileDataEpisodes);
    Task<FileDataEntity?> ObterArquivoCompleto(string processId, int? pageNumber, int? pageSize);

}