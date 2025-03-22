using LogWorker.Services;
using System.Diagnostics;

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
            if (IsMTGAOpen())
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    _logger.LogInformation("Waking up MTGA log worker.");
                    var logReaderService = scope.ServiceProvider.GetRequiredService<ILogReaderService>();
                    await logReaderService.ProcessLogFile();                    
                }
            }
            else
            {
                _logger.LogInformation("MTGA process not detected. Skipping log reader.");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }

    private bool IsMTGAOpen()
    {
        //return Process.GetProcessesByName("MTGA").Any();
        return true;
    }
}
