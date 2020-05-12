using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using WindowsService.HttpWebServer.Common;

namespace WindowsService.HttpWebServer.Handler
{
    public class LogicHandler
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
     
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="param">http请求传来的参数</param>
        public void FileUpload(Dictionary<string, string> param)
        {
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Content-Type", "multipart/form-data");
            //传递参数
            request.AddParameter("", "");

            //文件路径
            request.AddFile("file1", "filePath");
            request.AddFile("file2", "filePath2");
            request.Timeout = 10 * 60 * 1000;
            var restClient = new RestClient(Config.UploadApi);

            restClient.ExecuteAsync(request, (response) =>
          {
              if (logger.IsInfoEnabled)
              {
                  logger.Info(response.Content);
              }
              if (response.StatusCode == HttpStatusCode.OK)
              {
                  logger.Info("上传成功！");
              }
              else
              {
                  logger.Error("上传失败：" + response.ErrorMessage, response.ErrorException);
              }
          });

        }
    }
}
