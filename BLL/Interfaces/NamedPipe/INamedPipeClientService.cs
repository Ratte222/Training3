using AuxiliaryLib.Helpers;
using DAL.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace BLL.Interfaces.NamedPipe
{
    public interface INamedPipeClientService
    {
        bool Re_sendProblemNotifications();
        bool SendNotification(Notification notification);
        bool UpdateProblemNotification(Notification notification);
        PageResponse<Notification> CheckProblemNotification(int? pageLength = null, int? pageNumber = null);
    }
}
