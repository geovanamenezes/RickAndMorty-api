namespace QueueConsumer.Controller;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;
using System.IO;
using CompleteFile.UsecaseInterface;
using Correlation.Services;


public class QueueConsumerController : BackgroundService
{
    private readonly IServiceProvider serviceProvider;
    private readonly ICorrelationService correlationService;
    private readonly ILogger<QueueConsumerController> logger;
    public QueueConsumerController(IServiceProvider _serviceProvider, ICorrelationService _correlationService, ILogger<QueueConsumerController> _logger)
    {
        serviceProvider = _serviceProvider;
        correlationService = _correlationService;
        logger = _logger;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var reader = IdProcessorQueue.Queue.Reader;

        while (await reader.WaitToReadAsync(stoppingToken))
        {
            while (reader.TryRead(out var processId))
            {
                correlationService.SetCorrelationId(Guid.Parse(processId));
                logger.LogInformation($"Iniciando processamento do arquivo: {processId}.");
                using var scope = serviceProvider.CreateScope();
                var completeFileUseCase = scope.ServiceProvider.GetRequiredService<ICompleteFile>();
                try
                {
                    var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                    logger.LogInformation($"Arquivo a ser buscado para processamento: {processId}");
                    await Task.Delay(1000);

                    var filePath = Path.Combine(uploadsDir, processId);

                    if (!File.Exists(filePath))
                    {
                        logger.LogWarning($"Arquivo não encontrado: {processId}");
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
                        logger.LogWarning($"Nenhum registro válido encontrado para o arquivo {processId}.");
                        continue;
                    }
                    else
                    {
                        var resultado = await completeFileUseCase.CompletaArquivo(registros, processId );
                        logger.LogInformation($"Arquivo {processId} completado com sucesso.");
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError($"Erro ao processar {processId}: {ex.Message}");
                }
            }
        }
    }
}
