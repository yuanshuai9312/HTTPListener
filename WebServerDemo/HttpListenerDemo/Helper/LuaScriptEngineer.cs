using CommonLib.LogOperation;
using HttpListenerDemo.Test;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HttpListenerDemo.Helper
{
    public class LuaScriptEngineer
    {
        public string _scriptRoot;
        private string _throwMessage = "";
        private ReturnCode _returnCode = null;

        public void ExecuteScript(string scriptName, NameValueCollection parameters)
        {
            if (!File.Exists(Path.Combine(_scriptRoot, scriptName + ".lua")))
            {
                throw new FileNotFoundException();
            }

            LuaApiRegister luaHelper = new LuaApiRegister(new TestLuaApiInterface());

            InitLuaGlobalParameter(luaHelper, parameters);

            ExecuteFile(luaHelper, Path.Combine(_scriptRoot, scriptName + ".lua"));

        }

        private void InitLuaGlobalParameter(LuaApiRegister luaHelper, NameValueCollection parameters)
        {
            foreach (var item in parameters.AllKeys)
            {
                luaHelper.ExecuteString("a_" + item.Trim() + " = \"" + parameters[item].Replace("\\", "\\\\") + "\";");
            }
        }

        private void ExecuteFile(LuaApiRegister luaHelper, string luaFileName)
        {
            try
            {
                _throwMessage = "";
                _returnCode = null;
                luaHelper.ExecuteFile(luaFileName);
            }
            catch (ReturnCode returnCode)
            {
                _returnCode = returnCode;
            }
            catch (Exception ex)
            {
                _throwMessage = ex.Message;
            }

            if (_returnCode != null)
            {
                throw _returnCode;
            }
            else if (string.IsNullOrEmpty(_throwMessage))
            {
                Logger.Info("脚本执行完毕：" + luaFileName);
            }
            else if (!string.IsNullOrEmpty(_throwMessage))
            {
                Logger.Error(_throwMessage);
                throw new Exception(_throwMessage);
            }

        }

        public void Start()
        {
            _scriptRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Script");

            Logger.Info("脚本路径-" + _scriptRoot);
            if (!Directory.Exists(_scriptRoot))
            {
                Logger.Error("脚本根路径不存在！");
            }

            if (File.Exists(_scriptRoot + "Startup.lua"))
            {
                Logger.Info("开始执行初始化脚本！");
                try
                {
                    ExecuteScript("Startup", new NameValueCollection());
                }
                catch
                {
                    Logger.Error("启动初始化脚本失败！");
                }
            }
        }

        public void Stop()
        {
            try
            {
                Logger.Info("开始执行回收资源脚本！");
                ExecuteScript("Cleanup", new NameValueCollection());//清空所调用的资源
            }
            catch
            {
                Logger.Warning("回收资源过程出错");
            }
        }

    }
}
