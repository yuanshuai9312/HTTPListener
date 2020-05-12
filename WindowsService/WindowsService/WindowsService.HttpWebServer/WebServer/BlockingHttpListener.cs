using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WindowsService.HttpWebServer.WebServer
{
    public class BlockingHttpListener : AbstractHttpListener
    {
        public BlockingHttpListener(List<string> urls, Action<Dictionary<string, string>> callback) : base(urls, callback)
        {

        }

        public override void Start()
        {
            Init();
            while (true)
            {
                //等待请求连接
                //没有请求则GetContext处于阻塞状态
                HttpListenerContext context = httpListener.GetContext();
                ThreadPool.QueueUserWorkItem(ListenerCallback, context);
            }
        }
        public void ListenerCallback(object obj)
        {
            HttpListenerContext context = (HttpListenerContext)obj;
            Handler(context);

        }
    }
}
