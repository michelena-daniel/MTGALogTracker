using LogWorker.Services;

namespace LogWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                _logger.LogInformation("Waking up MTGA log worker.");               
                var logReaderService = scope.ServiceProvider.GetRequiredService<ILogReaderService>();
                await logReaderService.ProcessLogFile();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
