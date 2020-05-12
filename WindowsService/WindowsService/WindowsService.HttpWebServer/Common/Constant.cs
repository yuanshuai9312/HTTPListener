using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace WindowsService.HttpWebServer.Common
{
    public class Constant
    {
        /// <summary>
        /// windows服务名称
        /// </summary>
        public const string WINDOWS_SERVICE_NAME = "HttpWebServer";

        /// <summary>
        /// windows服务路径
        /// </summary>
        public static readonly string WINDOWS_SERVICE_PATH = $"{System.Environment.CurrentDirectory}\\WindowsService.HttpWebServer.exe";
    }
}
