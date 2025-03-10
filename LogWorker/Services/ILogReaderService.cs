using Domain.Models;

namespace LogWorker.Services
{
    public interface ILogReaderService
    {
        Task ProcessLogFile();
    }
}
