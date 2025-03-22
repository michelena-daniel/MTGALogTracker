using Domain.Interfaces;
using Domain.Models.Settings;
using Microsoft.Extensions.Options;

namespace LogWorker.Services.Downloader
{
    public class MtgaJsonService : IMtgaJsonService
    {
        private readonly MtgaJsonOptions _options;
        private readonly ILogger<MtgaJsonService> _logger;
        private readonly HttpClient _httpClient;

        public MtgaJsonService(IOptions<MtgaJsonOptions> options, ILogger<MtgaJsonService> logger)
        {
            _options = options.Value;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task DownloadMtgaJsonAsync()
        {
            try
            {
                _logger.LogInformation("Downloading new MTGA.json from {url}", _options.DownloadUrl);

                var response = await _httpClient.GetAsync(_options.DownloadUrl);
                response.EnsureSuccessStatusCode();

                var jsonContent = await response.Content.ReadAsStringAsync();

                var directory = Path.GetDirectoryName(_options.Path);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                await File.WriteAllTextAsync(_options.Path, jsonContent);

                _logger.LogInformation("AllPrintings.json.zip downloaded to {path}", _options.Path);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download AllPrintings.json.zip");
            }            
        }

        public bool ShouldDownloadMtgaJson()
        {
            if (!File.Exists(_options.Path))
                return true;

            var lastWriteTime = File.GetLastWriteTimeUtc(_options.Path);
            return (DateTime.UtcNow - lastWriteTime).TotalDays > 7;
        }
    }
}
