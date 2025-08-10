using File.Ports;
using Status.Ports;
using UploadHistory.Usecase;
using UploadHistory.UsecaseInterface;
using UploadHistory.RepositoryInterface;
using UploadHistory.Repository;
using QueueConsumer.Controller;
using CompleteFile.Usecase;
using CompleteFile.UsecaseInterface;
using Serilog;
using Serilog.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configura logging e OpenAPI
builder.Services.AddOpenApi();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Detecta variável HOME para definir caminho do banco
string? homePath = Environment.GetEnvironmentVariable("HOME");

string dbPath = !string.IsNullOrEmpty(homePath)
    ? Path.Combine(homePath, "uploadhistory.db")  // Azure / ambiente com HOME definido
    : "uploadhistory.db";                         // local

var connectionString = $"Data Source={dbPath}";

// Registra o DbContext com SQLite
builder.Services.AddDbContext<UploadHistoryContext>(options =>
    options.UseSqlite(connectionString));

// Registra serviços e UseCases
builder.Services.AddScoped<IUploadHistory, UploadHistoryUseCase>();
builder.Services.AddScoped<ICompleteFile, CompleteFileUseCase>();
builder.Services.AddScoped<IUploadHistoryRepository, UploadHistoryRepository>();
builder.Services.AddHostedService<QueueConsumerController>();

// Configura Serilog
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
);

// Configura Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5164);
});

var app = builder.Build();

// Aplica migrations automaticamente no startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UploadHistoryContext>();
    db.Database.Migrate();
}

// Mapeia OpenAPI só no desenvolvimento
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Middleware para CorrelationId
app.Use(async (context, next) =>
{
    var correlationId = Guid.NewGuid().ToString();
    LogContext.PushProperty("CorrelationId", correlationId);
    context.Response.Headers["X-Correlation-ID"] = correlationId;
    await next();
});

app.UseHttpsRedirection();

app.FilePort();
app.StatusPort();

app.Run();