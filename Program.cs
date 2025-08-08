
using File.Ports;
using Status.Ports;
using UploadHistory.Usecase;
using UploadHistory.UsecaseInterface;
using Microsoft.AspNetCore.Http.Features;
using UploadHistory.RepositoryInterface;
using UploadHistory.Repository;
using QueueConsumer.Controller;
using CompleteFile.Usecase;
using CompleteFile.UsecaseInterface;
using Serilog;
using Correlation.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 
builder.Services.AddScoped<UploadHistoryContext>();
builder.Services.AddScoped<IUploadHistory, UploadHistoryUseCase>();
builder.Services.AddScoped<ICompleteFile, CompleteFileUseCase>();
builder.Services.AddScoped<IUploadHistoryRepository, UploadHistoryRepository>();
builder.Services.AddHostedService<QueueConsumerController>();
builder.Services.AddSingleton<ICorrelationService, CorrelationService>();
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
);
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5164);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.FilePort();
app.StatusPort();

app.Run();
