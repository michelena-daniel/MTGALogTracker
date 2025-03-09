using Domain.Models;

namespace LogWorker.Services
{
    public interface ILogReaderService
    {
        void ProcessLogFile();
    }
}
