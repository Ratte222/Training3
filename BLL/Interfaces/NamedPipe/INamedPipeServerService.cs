using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces.NamedPipe
{
    public interface INamedPipeServerService
    {
        void StartService();
        void Stop();
    }
}
