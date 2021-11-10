using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmailService
    {
        Task SendAsync(string to, string heading, string body);
        void Send(string to, string heading, string body);
    }
}
