
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


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddScoped<UploadHistoryContext>();
builder.Services.AddScoped<IUploadHistory, UploadHistoryUseCase>();
builder.Services.AddScoped<ICompleteFile, CompleteFileUseCase>();
builder.Services.AddScoped<IUploadHistoryRepository, UploadHistoryRepository>();
builder.Services.AddHostedService<QueueConsumerController>();


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
