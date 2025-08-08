using CompleteFile.UsecaseInterface;
using Serilog;
using System.IO;

namespace QueueConsumer.Controller;


public class QueueConsumerController : BackgroundService
{
    private readonly IServiceProvider serviceProvider;

    public QueueConsumerController(IServiceProvider _serviceProvider)
    {
        serviceProvider = _serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = IdProcessorQueue.Queue.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var processId))
            {
                Log.Information($"Iniciando processamento do arquivo: {processId}.");
                using var scope = serviceProvider.CreateScope();
                var completeFileUseCase = scope.ServiceProvider.GetRequiredService<ICompleteFile>();
                try
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    Log.Information($"Arquivo a ser buscado para processamento: {processId}");
                    await Task.Delay(1000);

                    var filePath = Path.Combine(uploadsDir, processId);

                    if (!System.IO.File.Exists(filePath))
                    {
                        Log.Warning($"Arquivo não encontrado: {processId}");
                        continue;
                    }

                    var lines = await System.IO.File.ReadAllLinesAsync(filePath, stoppingToken);
                    int lineNumber = 0;
                    var registros = new List<ContentFileEntity>();

                    foreach (var line in lines)
                    {
                        lineNumber++;

                        if (lineNumber == 1)
                            continue;

                        var parts = line.Split(',');

                        var registro = new ContentFileEntity
                        {
                            EpisodeId = int.TryParse(parts[0], out var ep) ? ep : 0,
                            CharacterId = int.TryParse(parts[1], out var ch) ? ch : 0,
                            CharacterName = parts[2],
                            LocationId = int.TryParse(parts[3], out var loc) ? loc : 0
                        };

                        registros.Add(registro);
                    }

                    if (registros == null || !registros.Any())
                    {
                        Log.Warning($"Nenhum registro válido encontrado para o arquivo {processId}.");
                        continue;
                    }
                    else
                    {
                        var resultado = await completeFileUseCase.CompletaArquivo(registros, processId );
                        Log.Information($"Arquivo {processId} completado com sucesso.");
                    }

                }
                catch (Exception ex)
                {
                    Log.Error($"Erro ao processar {processId}: {ex.Message}");
                }
            }
        }
    }
}
