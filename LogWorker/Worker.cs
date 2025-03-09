using LogWorker.Services;

namespace LogWorker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private ILogReaderService _logReaderService;

    public Worker(ILogger<Worker> logger, ILogReaderService logReaderService)
    {
        _logger = logger;
        _logReaderService = logReaderService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logReaderService.ProcessLogFile();
            await Task.Delay(1000, stoppingToken);
        }
    }
}
