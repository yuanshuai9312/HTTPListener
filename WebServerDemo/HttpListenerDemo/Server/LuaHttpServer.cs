using CommonLib.LogOperation;
using HttpListenerDemo.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HttpListenerDemo.Server
{
    class LuaHttpServer : HttpImplanter
    {
        LuaScriptEngineer _luaScriptEngineer = new LuaScriptEngineer();

        #region HttpImplanter 成员

        public void Start()
        {
            _luaScriptEngineer.Start();
        }

        public void Stop()
        {
            _luaScriptEngineer.Stop();
        }

        public void MakeHttpPrefix(System.Net.HttpListener server)
        {
            server.Prefixes.Clear();
            server.Prefixes.Add("http://localhost:8082/");
            Logger.Info("LuaHttpServer添加监听前缀：http://localhost:8082/");
        }

        public ReturnCode ProcessRequest(System.Net.HttpListenerContext context)
        {
            UrlHelper urlHelper = new UrlHelper(context.Request.Url);
            CommandResult result = urlHelper.ParseResult;
            if (urlHelper.ParseResult == CommandResult.Success)
            {
                try
                {
                    _luaScriptEngineer.ExecuteScript(urlHelper.ScriptName, urlHelper.Parameters);
                    return new ReturnCode((int)CommandResult.Success, EnumHelper.GetEnumDescription(CommandResult.Success));
                }
                catch (FileNotFoundException fileNotFoundException)
                {
                    return new ReturnCode((int)CommandResult.NoExistsMethod, EnumHelper.GetEnumDescription(CommandResult.NoExistsMethod));
                }
                catch (ReturnCode returnCode)
                {
                    return returnCode;
                }
                catch (Exception ex)
                {
                    return new ReturnCode((int)CommandResult.ExcuteFunctionFailed, EnumHelper.GetEnumDescription(CommandResult.ExcuteFunctionFailed));
                }
            }
            return new ReturnCode((int)result, EnumHelper.GetEnumDescription(result));
        }

        public byte[] CreateReturnResult(System.Net.HttpListenerContext context, ReturnCode result)
        {
            string responseString = string.Format("code={0}&msg={1}&request={2}",
                result.Code,
                result.Message,
                context.Request.Url.ToString()
                );

            return System.Text.Encoding.UTF8.GetBytes(responseString);
        }

        #endregion
    }
}
