using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces.NamedPipe
{
    public interface INamedPipeClientService
    {
        bool SendNotification(Notification notification);
    }
}
