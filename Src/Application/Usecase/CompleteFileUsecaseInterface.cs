namespace CompleteFile.UsecaseInterface;
public interface ICompleteFile
{
    Task<FileDataEntity> CompletaArquivo(List<ContentFileEntity> contentsFile, string processId);
}