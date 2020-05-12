using CommonLib.LogOperation;
using HttpListenerDemo.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpListenerDemo.Server
{
    class MVCHttpServer : HttpImplanter
    {
        string _route = null;
        int _controllerIndex = -1;
        int _actionIndex = -1;

        #region HttpImplanter 成员

        public void Start()
        {
            _route = new TestMVCRoute().RegisterRoutes().FirstOrDefault();

            var routes = _route.Split('/');
            for (int i = 0; i < routes.Length; i++)
            {
                if (routes[i] == "{controller}")
                {
                    _controllerIndex = i;
                }
                else if (routes[i] == "{action}")
                {
                    _actionIndex = i;
                }
            }
        }

        public void Stop()
        {
            //nothing to do
        }

        public void MakeHttpPrefix(System.Net.HttpListener server)
        {
            server.Prefixes.Clear();
            server.Prefixes.Add("http://localhost:8083/");
            Logger.Info("MVCHttpServer添加监听前缀：http://localhost:8083/");
        }

        public ReturnCode ProcessRequest(System.Net.HttpListenerContext context)
        {
            return new ReturnCode((int)CommandResult.Success, EnumHelper.GetEnumDescription(CommandResult.Success));
        }

        public byte[] CreateReturnResult(System.Net.HttpListenerContext context, ReturnCode result)
        {
            string responseString = string.Empty;
            var splitedPath = context.Request.Url.AbsolutePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var controllerName = splitedPath[_controllerIndex] + "Controller";
            var actionName = splitedPath[_actionIndex];

            var type = Type.GetType("HttpListenerDemo.Test." + controllerName);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type);
                responseString = obj.GetType().GetMethod(actionName).Invoke(obj, null) as string;
            }

            return System.Text.Encoding.UTF8.GetBytes(responseString);
        }

        #endregion
    }
}
