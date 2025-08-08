namespace QueueConsumer.Controller;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using System.IO;
using CompleteFile.UsecaseInterface;


public class QueueConsumerController : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public QueueConsumerController(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = IdProcessorQueue.Queue.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var processId))
            {
                using var scope = _serviceProvider.CreateScope();
                var completeFileUseCase = scope.ServiceProvider.GetRequiredService<ICompleteFile>();
                try
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    Console.WriteLine($"uploadsDir: {uploadsDir}");

                    Console.WriteLine($"[BG Service] Processando: {processId}");
                    await Task.Delay(1000);

                    var filePath = Path.Combine(uploadsDir, processId);
                    Console.WriteLine($"filePath: {filePath}");

                    if (!File.Exists(filePath))
                    {
                        Console.WriteLine($"Arquivo nao encontrado: {processId}");
                        continue;
                    }

                    var lines = await File.ReadAllLinesAsync(filePath, stoppingToken);
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
                        Console.WriteLine($"Nenhum registro válido encontrado para o arquivo {processId}.");
                        continue;
                    }
                    else
                    {
                        var resultado = await completeFileUseCase.CompletaArquivo(registros, processId );
                        Console.WriteLine($"Arquivo {processId} completado com sucesso.");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao processar {processId}: {ex.Message}");
                }
            }
        }
    }
}
