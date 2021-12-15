using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = PathToNotificationService;
            startInfo.CreateNoWindow = false;
            //startInfo.UseShellExecute = false;
            //startInfo.RedirectStandardOutput = true;
            _process.StartInfo = startInfo;
        }

        public bool Start()
        {            
            if(_process != null)
            {                
                return _process.Start();
            }
            return false;
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
