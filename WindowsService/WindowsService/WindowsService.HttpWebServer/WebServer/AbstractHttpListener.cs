using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsService.HttpWebServer.WebServer
{
    public abstract class AbstractHttpListener
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected HttpListener httpListener;

        public List<string> Urls { get; set; }

        public Action<Dictionary<string, string>> Callback { get; set; }

        public AbstractHttpListener(List<string> urls, Action<Dictionary<string, string>> callback)
        {
            this.Urls = urls;
            this.Callback = callback;
        }

        protected void Init()
        {
            httpListener = new HttpListener();
            httpListener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            foreach (string url in Urls)
            {
                httpListener.Prefixes.Add(url);
            }
            httpListener.Start();
            if (logger.IsInfoEnabled)
            {
                logger.Info("监听http地址：" + string.Join(",", Urls));
                logger.Info("HttpListener启动！");
            }
        }

        protected void Handler(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            Dictionary<string, string> param = new Dictionary<string, string>();
            try
            {
                switch (request.HttpMethod)
                {
                    case "GET":
                        foreach (string key in request.QueryString.AllKeys)
                        {
                            param.Add(key, request.QueryString[key]);
                        }
                        break;
                    case "POST":
                        Stream stream = context.Request.InputStream;
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string content = reader.ReadToEnd();
                        param = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
                        break;
                    default:
                        throw new Exception("不支持的httpMethod=" + request.HttpMethod);

                }
                if (logger.IsInfoEnabled)
                {
                    logger.InfoFormat("HttpMethod={0},参数={1}", request.HttpMethod, JsonConvert.SerializeObject(param));
                }
                //处理逻辑
                Callback(param);

                Output(response, 100, "success!");
            }
            catch (Exception e)
            {
                Output(response, 500, e.Message);
                logger.Error("接收请求处理出错：", e);
            }
            finally
            {
                response.Close();
            }

        }

        private void Output(HttpListenerResponse response, int statusCode, string message)
        {
            //构造Response响应
            response.StatusCode = 200;
            response.ContentType = "application/json;charset=UTF-8";
            response.ContentEncoding = Encoding.UTF8;
            response.AppendHeader("Content-Type", "application/json;charset=UTF-8");

            using (StreamWriter writer = new StreamWriter(response.OutputStream, Encoding.UTF8))
            {
                var result = new { result = statusCode, message = message };
                writer.Write(JsonConvert.SerializeObject(result));
                writer.Close();
            }
        }

        public abstract void Start();

        public virtual void Close()
        {
            if (httpListener != null)
            {
                httpListener.Close();
                if (logger.IsInfoEnabled)
                {
                    logger.Info("HttpListener Close!");
                }
            }
        }
    }
}
