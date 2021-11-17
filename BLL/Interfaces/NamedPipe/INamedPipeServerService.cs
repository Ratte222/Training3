using BLL.Helpers;

namespace BLL.Interfaces.NamedPipe
{
    public interface INamedPipeServerService
    {
        void StartService(IncomingDataForPipeServer incomingData, int numThreads);
        void Stop();
    }
}
