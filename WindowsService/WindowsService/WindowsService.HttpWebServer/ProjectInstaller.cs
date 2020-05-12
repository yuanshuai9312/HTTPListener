using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using WindowsService.HttpWebServer.Common;

namespace WindowsService.HttpWebServer
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 重写Commit方法,安装后自动启动
        /// </summary>
        /// <param name="savedState"></param>
        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            ServiceController sc = new ServiceController(Constant.WINDOWS_SERVICE_NAME);
            if (sc.Status.Equals(ServiceControllerStatus.Stopped))
            {
                sc.Start();
            }
        }

        public override void Uninstall(IDictionary savedState)
        {
            ServiceController sc = new ServiceController(Constant.WINDOWS_SERVICE_NAME);
            if (sc.Status.Equals(ServiceControllerStatus.Running))
            {
                sc.Stop();
            }
            base.Uninstall(savedState);
        }
    }
}
