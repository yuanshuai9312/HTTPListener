using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WindowsService.HttpWebServer.Common;
using WindowsService.HttpWebServer.Handler;
using WindowsService.HttpWebServer.WebServer;

namespace WindowsService.HttpWebServer
{
    public partial class LocalWebServer : ServiceBase
    {
        private NonblockingHttpListener httpListener;
        public LocalWebServer()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            LogicHandler handler = new LogicHandler();
            httpListener = new NonblockingHttpListener(Config.Urls, handler.FileUpload);
            httpListener.Start();
        }

        protected override void OnStop()
        {
            if (httpListener != null)
            {
                httpListener.Close();
            }
        }
    }
}
