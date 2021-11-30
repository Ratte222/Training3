using NotificationService.Helpers;
using System.Threading.Tasks;

namespace NotificationService.Interfaces.NamedPipe
{
    public interface INamedPipeServerService
    {
        Task StartServiceAsync(IncomingDataForPipeServer incomingData, int numThreads);
        void Stop();
    }
}
