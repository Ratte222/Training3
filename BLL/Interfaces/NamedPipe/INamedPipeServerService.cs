using BLL.Helpers;
using System.Threading.Tasks;

namespace BLL.Interfaces.NamedPipe
{
    public interface INamedPipeServerService
    {
        Task StartServiceAsync(IncomingDataForPipeServer incomingData, int numThreads);
        void Stop();
    }
}
