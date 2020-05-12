using CommonLib.LogOperation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace HttpListenerDemo
{
    /// <summary>
    /// 返回执行结果的枚举
    /// </summary>
    public enum CommandResult
    {
        [Description("成功")]
        Success = 1,

        [Description("Url太长")]
        UrlTooLong = 2,

        [Description("Url包含特殊字符")]
        UrlInvalidChar = 3,

        [Description("文件名含特殊字符")]
        FileNameInvalidChar = 4,

        [Description("方法不存在")]
        NoExistsMethod = 5,

        [Description("文件不存在")]
        FileNotExists = 6,

        [Description("执行功能失败")]
        ExcuteFunctionFailed = 7,

        [Description("服务器正忙")]
        ServerIsBusy = 8,

        [Description("不支持的请求文件类型")]
        NotSupportRequestedFileType = 9,
    };

    /// <summary>
    /// 实现HttpServer要支持的接口
    /// </summary>
    interface HttpImplanter
    {
        void Start();
        void Stop();
        void MakeHttpPrefix(HttpListener server);
        ReturnCode ProcessRequest(HttpListenerContext context);
        byte[] CreateReturnResult(HttpListenerContext context, ReturnCode result);
    }

    /// <summary>
    /// 可接收Http请求的服务器
    /// </summary>
    class HttpServer
    {
        Thread _httpListenThread;

        /// <summary>
        /// HttpServer是否已经启动
        /// </summary>
        volatile bool _isStarted = false;

        /// <summary>
        /// 线程是否已经结束
        /// </summary>
        volatile bool _terminated = false;
        volatile bool _ready = false;
        volatile bool _isRuning = false;
        HttpImplanter _httpImplanter;

        public void Start(HttpImplanter httpImplanter)
        {
            if (!HttpListener.IsSupported)
            {
                Logger.Exit("不支持HttpListener!");
            }

            if (_isStarted)
            {
                return;
            }
            _isStarted = true;
            _ready = false;
            _httpImplanter = httpImplanter; 

            RunHttpServerThread();

            while (!_ready) ;
        }

        private void RunHttpServerThread()
        {
            _httpListenThread = new Thread(new ThreadStart(() =>
            {
                HttpListener server = new HttpListener();
                try
                {
                    _httpImplanter.MakeHttpPrefix(server);
                    server.Start();
                }
                catch (Exception ex)
                {
                    Logger.Exit("无法启动服务器监听，请检查网络环境。");
                }

                _httpImplanter.Start();

                IAsyncResult result = null;
                while (!_terminated)
                {
                    while (result == null || result.IsCompleted)
                    {
                        result = server.BeginGetContext(new AsyncCallback(ProcessHttpRequest), server);
                    }
                    _ready = true;
                    Thread.Sleep(10);
                }

                server.Stop();
                server.Abort();
                server.Close();
                _httpImplanter.Stop();
            }
            ));

            _httpListenThread.IsBackground = true;
            _httpListenThread.Start();
        }

        private void ProcessHttpRequest(IAsyncResult iaServer)
        {
            HttpListener server = iaServer.AsyncState as HttpListener;
            HttpListenerContext context = null;
            try
            {
                context = server.EndGetContext(iaServer);
                Logger.Info("接收请求" + context.Request.Url.ToString());
                //判断上一个操作未完成，即返回服务器正忙，并开启一个新的异步监听
                if (_isRuning)
                {
                    Logger.Info("正在处理请求，已忽略请求" + context.Request.Url.ToString());
                    RetutnResponse(context, _httpImplanter.CreateReturnResult(context, new ReturnCode((int)CommandResult.ServerIsBusy, EnumHelper.GetEnumDescription(CommandResult.ServerIsBusy))));
                    server.BeginGetContext(new AsyncCallback(ProcessHttpRequest), server);
                    return;
                }

                _isRuning = true;
                server.BeginGetContext(new AsyncCallback(ProcessHttpRequest), server);
            }
            catch
            {
                Logger.Warning("服务器已关闭！");
                return;
            }

            string scriptName = new UrlHelper(context.Request.Url).ScriptName;
            byte[] resultBytes = null;
            if (scriptName.ToLower().EndsWith(".html")||scriptName == "favicon.ico")
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web", scriptName);
                if (File.Exists(filePath))
                {
                    resultBytes = File.ReadAllBytes(filePath);
                }
                else
                {
                    resultBytes = _httpImplanter.CreateReturnResult(context, new ReturnCode((int)CommandResult.FileNotExists, EnumHelper.GetEnumDescription(CommandResult.FileNotExists)));
                }
            }
            else
            {
                ReturnCode result = _httpImplanter.ProcessRequest(context);
                resultBytes = _httpImplanter.CreateReturnResult(context, result);
            }
            RetutnResponse(context, resultBytes);
            _isRuning = false;
        }

        private static void RetutnResponse(HttpListenerContext context, byte[] resultBytes)
        {
            context.Response.ContentLength64 = resultBytes.Length;
            System.IO.Stream output = context.Response.OutputStream;
            try
            {
                output.Write(resultBytes, 0, resultBytes.Length);
                output.Close();
            }
            catch
            {
                Logger.Warning("客户端已经关闭!");
            }
        }

        public void Stop()
        {
            if (!_isStarted)
            {
                return;
            }

            _terminated = true;
            _httpListenThread.Join();

            _isStarted = false;
        }

    }
}
