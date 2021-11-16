using System;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IProblemNotificationsService:IDisposable
    {
        void Stop();
        Task Execute();
    }
}