using Domain.Models.Settings;
using LogWorker;
using LogWorker.Services;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

builder.Services.Configure<LogPathOptions>(
    builder.Configuration.GetSection("LogPath"));
builder.Services.AddTransient<ILogReaderService, LogReaderService>();

var host = builder.Build();
host.Run();
