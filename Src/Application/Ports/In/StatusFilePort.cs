using UploadHistory.UsecaseInterface;
using Correlation.Services;
namespace Status.Ports;
public static class UploadHistoryPorts
{
    public static void StatusPort(this WebApplication app)
    {
        app.MapGet("/status/{processId}", async (
            string processId,
            IUploadHistory useCase,
            ICorrelationService correlationService,
            ILogger logger) =>
        {
            correlationService.SetCorrelationId(Guid.Parse(processId));
            logger.LogInformation($"Buscando status do arquivo: {processId}");
            var result = await useCase.BuscaStatusArquivo(processId);
            return Results.Ok(result);
        });


    }
}