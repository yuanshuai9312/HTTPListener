using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService.HttpWebServer.Common
{
    public static class Config
    {
        /// <summary>
        /// 监听的http请求地址
        /// </summary>
        private static readonly string urls = ConfigurationManager.AppSettings["Urls"];
        /// <summary>
        /// 如果没有配置默认监听的地址
        /// </summary>
        private static readonly string defaultUrl = "http://127.0.0.1:8888/service/";
        /// <summary>
        /// 文件目录
        /// </summary>
        public static readonly string FileDir = ConfigurationManager.AppSettings["FileDir"];
        /// <summary>
        /// 文件上传api
        /// </summary>
        public static readonly string UploadApi = ConfigurationManager.AppSettings["UploadApi"];


        public static List<string> Urls
        {
            get
            {
                List<string> list = new List<string>();
                if (string.IsNullOrWhiteSpace(urls))
                {
                    list.Add(defaultUrl);
                    return list;
                }
                string[] arr = urls.Split(',');
                list.AddRange(arr);
                return list;
            }
        }
    }
}
