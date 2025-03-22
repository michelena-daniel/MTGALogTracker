namespace Domain.Interfaces
{
    public interface IMtgaJsonService
    {
        Task DownloadMtgaJsonAsync();
        bool ShouldDownloadMtgaJson();
    }
}
