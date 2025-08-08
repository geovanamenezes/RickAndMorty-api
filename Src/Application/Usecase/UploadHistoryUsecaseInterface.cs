namespace UploadHistory.UsecaseInterface;
using ReceivedFile.Entity;

public interface IUploadHistory
{
    Task<string> ProcessaArquivo(IFormFile file);
    Task<ReceivedFileEntity?> BuscaStatusArquivo(string processId);
    Task<FileDataTO?> RetornaDadosArquivoCompleto(string processId, int? pageNumber, int? pageSize, string? searchTerm, string? orderByName);
}