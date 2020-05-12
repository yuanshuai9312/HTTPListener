using CommonLib.LogOperation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpListenerDemo.Server
{
    public class MethodHttpServer : HttpImplanter
    {
        #region HttpImplanter 成员

        public void Start()
        {
            //nothing to do
        }

        public void Stop()
        {
            //nothing to do
        }

        public void MakeHttpPrefix(System.Net.HttpListener server)
        {
            server.Prefixes.Clear();
            server.Prefixes.Add("http://localhost:8081/");
            Logger.Info("MethodHttpServer添加监听前缀：http://localhost:8081/");
        }

        public ReturnCode ProcessRequest(System.Net.HttpListenerContext context)
        {
            return new ReturnCode((int)CommandResult.Success, EnumHelper.GetEnumDescription(CommandResult.Success));
        }

        public byte[] CreateReturnResult(System.Net.HttpListenerContext context, ReturnCode result)
        {
            string responseString = string.Empty;
            UrlHelper urlHelper = new UrlHelper(context.Request.Url);
            var type = Type.GetType("HttpListenerDemo.Test." + urlHelper.ScriptName);
            if (type != null)
            {
                object obj = Activator.CreateInstance(type);
                responseString = obj.GetType().GetMethod(urlHelper.Parameters["method"]).Invoke(obj, new object[] { urlHelper.Parameters["param"] }) as string;
            }

            return System.Text.Encoding.UTF8.GetBytes(responseString);
        }

        #endregion
    }
}
