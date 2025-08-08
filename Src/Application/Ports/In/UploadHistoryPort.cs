
using UploadHistory.UsecaseInterface;
using Correlation.Services;
namespace File.Ports;
public static class FilePorts
{
    public static void FilePort(this WebApplication app)
    {
        app.MapPost("/upload", async (
            HttpRequest request,
            IUploadHistory useCase,
            ILogger logger,
            ICorrelationService correlationService) =>
        {
            try
            {
                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");

                if (file is null || file.Length == 0)
                    return Results.BadRequest("O arquivo CSV não foi enviado ou está vazio.");

                var processId = await useCase.ProcessaArquivo(file);
                correlationService.SetCorrelationId(Guid.Parse(processId));
                logger.LogInformation($"Recebimento do arquivo {processId} com sucesso.");
                await IdProcessorQueue.Queue.Writer.WriteAsync(processId);
                logger.LogInformation($"Arquivo {processId} inserido na fila para posterior processamento.");

                return Results.Ok(new
                {
                    Message = "Upload realizado com sucesso!",
                    ProcessId = processId,
                    StatusUrl = $"/status/{processId}"
                });
            }
            catch (InvalidDataException ex)
            {
                logger.LogError($"Erro ao processar o arquivo: {ex.Message}");
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                logger.LogError($"Erro ao processar o arquivo: {errorMessage}");
                return Results.Problem("Erro interno ao processar o arquivo." + errorMessage);
            }
        });

        app.MapGet("/upload/{processId}", async (
            string processId,
            int? pageNumber,
            int? pageSize,
            string? searchTerm,
            string? orderBy,
            IUploadHistory useCase,
            ILogger logger,
            ICorrelationService correlationService) =>
        {
            try
            {
                correlationService.SetCorrelationId(Guid.Parse(processId));
                logger.LogInformation($"Buscando dados do arquivo: {processId}");
                var result = await useCase.RetornaDadosArquivoCompleto(processId, pageNumber, pageSize, searchTerm, orderBy);

                if (result == null)
                    return Results.NotFound($"Arquivo com ProcessId {processId} não encontrado.");

                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                logger.LogError($"Erro ao processar o arquivo: {errorMessage}");
                return Results.BadRequest(new { erro = errorMessage });

            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                logger.LogError($"Erro ao processar o arquivo: {errorMessage}");
                return Results.Problem("Erro interno ao processar o arquivo." + errorMessage);
            }

        });



    }
}