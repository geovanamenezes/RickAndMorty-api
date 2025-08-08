
namespace File.Ports;
using UploadHistory.UsecaseInterface;

public static class FilePorts
{
    public static void FilePort(this WebApplication app)
    {
        app.MapPost("/upload", async (HttpRequest request, IUploadHistory useCase) =>
        {
            try
            {
                var form = await request.ReadFormAsync();
                var file = form.Files.GetFile("file");

                if (file is null || file.Length == 0)
                    return Results.BadRequest("O arquivo CSV não foi enviado ou está vazio.");

                var processId = await useCase.ProcessaArquivo(file);
                await IdProcessorQueue.Queue.Writer.WriteAsync(processId);

                return Results.Ok(new
                {
                    Message = "Upload realizado com sucesso!",
                    ProcessId = processId,
                    StatusUrl = $"/status/{processId}"
                });
            }
            catch (InvalidDataException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;

                return Results.Problem("Erro interno ao processar o arquivo." + errorMessage);
            }
        });

        app.MapGet("/upload/{processId}", async (
            string processId,
            int? pageNumber,
            int? pageSize,
            string? searchTerm,
            string? orderBy,
            IUploadHistory useCase) =>
        {
            try
            {
                var result = await useCase.RetornaDadosArquivoCompleto(processId, pageNumber, pageSize, searchTerm, orderBy);

                if (result == null)
                    return Results.NotFound($"Arquivo com ProcessId {processId} não encontrado.");

                return Results.Ok(result);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { erro = ex.Message });

            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;

                return Results.Problem("Erro interno ao processar o arquivo." + errorMessage);
            }

        });



    }
}