
using UploadHistory.UsecaseInterface;

namespace Status.Ports;
public static class UploadHistoryPorts
{
    public static void StatusPort(this WebApplication app)
    {
        app.MapGet("/status/{processId}", async (
            string processId,
            IUploadHistory useCase) =>
        {
            var result = await useCase.BuscaStatusArquivo(processId);
            return Results.Ok(result);
        });


    }
}