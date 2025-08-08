
using UploadHistory.UsecaseInterface;
using Serilog;
namespace File.Ports;
public static class FilePorts
{
    public static void FilePort(this WebApplication app)
    {
        app.MapPost("/upload", async (
            HttpRequest request,
            IUploadHistory useCase) =>
        {
            try
            {
                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");

                if (file is null || file.Length == 0)
                    return Results.BadRequest("O arquivo CSV não foi enviado ou está vazio.");

                var processId = await useCase.ProcessaArquivo(file);
                Log.Information($"Recebimento do arquivo {processId} com sucesso.");
                await IdProcessorQueue.Queue.Writer.WriteAsync(processId);
                Log.Information($"Arquivo {processId} inserido na fila para posterior processamento.");

                return Results.Ok(new
                {
                    Message = "Upload realizado com sucesso!",
                    ProcessId = processId,
                    StatusUrl = $"/status/{processId}"
                });
            }
            catch (InvalidDataException ex)
            {
                Log.Error($"Erro ao processar o arquivo: {ex.Message}");
                return Results.BadRequest(new {error = ex.Message });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                Log.Error($"Erro ao processar o arquivo: {errorMessage}");
                return Results.Problem("Erro interno ao processar o arquivo." + errorMessage);
            }
        });

        app.MapGet("/upload/{processId}", async (
            string processId,
            string? pageNumber,
            string? pageSize,
            string? searchTerm,
            string? orderBy,
            IUploadHistory useCase) =>
        {
            try
            {
                int? page = int.TryParse(pageNumber, out var p) ? p : null;
                int? size = int.TryParse(pageSize, out var s) ? s : null;
                Log.Information($"Buscando dados do arquivo: {processId}");
                var result = await useCase.RetornaDadosArquivoCompleto(processId, page, size, searchTerm, orderBy);

                if (result == null)
                    return Results.NotFound(new { Error = $"Arquivo com ProcessId {processId} não encontrado." });

                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                Log.Error($"Erro ao processar o arquivo: {errorMessage}");
                return Results.BadRequest(new { Error = errorMessage });

            }
            catch (KeyNotFoundException ex)
            {
                Log.Error($"Erro ao buscar dados do arquivo. Detalhes: {ex.Message}");
                return Results.NotFound(new { Error = "Não foi encontrado arquivo para esse identificador." });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                Log.Error($"Erro ao processar o arquivo: {errorMessage}");
                return Results.Problem("Erro interno ao processar o arquivo.");
            }

        });



    }
}