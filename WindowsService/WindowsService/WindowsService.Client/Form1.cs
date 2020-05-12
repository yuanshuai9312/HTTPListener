using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsService.HttpWebServer.Common;
using WindowsService.HttpWebServer.Common.Util;

namespace WindowsService.Client
{
    public partial class Form1 : Form
    {
        string serviceFilePath = Constant.WINDOWS_SERVICE_PATH;//$"{Application.StartupPath}\\WindowsService.HttpWebServer.exe";
        string serviceName = Constant.WINDOWS_SERVICE_NAME;
        public Form1()
        {
            InitializeComponent();

        }



        private void btnInstall_Click(object sender, EventArgs e)
        {
            if (WindowsServiceUtil.IsExist(serviceName))
            {
                WindowsServiceUtil.UninstallService(serviceFilePath);
            }
            WindowsServiceUtil.InstallService(serviceFilePath);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!WindowsServiceUtil.IsExist(serviceName))
            {
                MessageBox.Show(serviceName + "服务不存在");
            }
            WindowsServiceUtil.ServiceStart(serviceName);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (!WindowsServiceUtil.IsExist(serviceName))
            {
                MessageBox.Show(serviceName + "服务不存在");
            }
            WindowsServiceUtil.ServiceStop(serviceName);
        }

        private void btnUninstall_Click(object sender, EventArgs e)
        {
            if (!WindowsServiceUtil.IsExist(serviceName))
            {
                MessageBox.Show(serviceName + "服务不存在");
            }
            WindowsServiceUtil.ServiceStop(serviceName);
            WindowsServiceUtil.UninstallService(serviceFilePath);
        }
    }
}
