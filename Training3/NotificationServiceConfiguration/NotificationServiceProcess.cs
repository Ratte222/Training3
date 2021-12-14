using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Training3.NotificationServiceConfiguration
{
    public class NotificationServiceProcess:IDisposable
    {
        private string PathToNotificationService { get; set; }
        private Process _process;

        public NotificationServiceProcess(string pathToNotificationService)
        {
            PathToNotificationService = pathToNotificationService;
            _process = new Process();
        }

        public void Start()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = PathToNotificationService;
            startInfo.UseShellExecute = false;
            if(_process != null)
            {
                _process.StartInfo = startInfo;
                _process.Start();
            }            
        }

        public void KillProcess()
        {
            _process?.Kill();
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            if (!disposed && Dispose(true))
            {
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        protected virtual bool Dispose(bool manual)
        {
            if (manual)
            {
                _process?.Dispose();
                _process = null;
            }
            return true;
        }

        ~NotificationServiceProcess()
        {
            disposed = disposed || Dispose(false);
        }

        #endregion IDisposable
    }
}
