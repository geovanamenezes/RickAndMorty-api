using UploadHistory.UsecaseInterface;
using Serilog;
namespace Status.Ports;

public static class UploadHistoryPorts
{
    public static void StatusPort(this WebApplication app)
    {
        app.MapGet("/status/{processId}", async (
            string processId,
            IUploadHistory useCase) =>
        {
            try
            {
                Log.Information($"Buscando status do arquivo: {processId}");
                var result = await useCase.BuscaStatusArquivo(processId);

                if (result == null)
                {
                    throw new KeyNotFoundException("Não foi encontrado arquivo para esse identificador.");
                }

                return Results.Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                Log.Error($"Erro ao buscar status do arquivo. Detalhes: {ex.Message}");
                return Results.NotFound(new { Error = "Não foi encontrado arquivo para esse identificador." });
            }
            catch (Exception ex)
            {
                Log.Error($"Erro ao buscar status do arquivo. Detalhes: {ex.Message}");
                return Results.Problem("Erro interno ao buscar status do arquivo.");
            }
        });
    }
}