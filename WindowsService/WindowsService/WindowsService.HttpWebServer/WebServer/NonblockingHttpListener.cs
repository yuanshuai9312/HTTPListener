using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService.HttpWebServer.WebServer
{
    public class NonblockingHttpListener : AbstractHttpListener
    {
        public NonblockingHttpListener(List<string> urls, Action<Dictionary<string, string>> callback) : base(urls, callback)
        {

        }

        public override void Start()
        {
            Init();
            httpListener.BeginGetContext(ListenerCallback, httpListener);
        }

        public void ListenerCallback(IAsyncResult result)
        {
            httpListener = result.AsyncState as HttpListener;
            HttpListenerContext context = httpListener.EndGetContext(result);
            Handler(context);
            httpListener.BeginGetContext(ListenerCallback, httpListener);
        }
    }
}
