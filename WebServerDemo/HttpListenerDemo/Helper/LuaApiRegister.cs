using CommonLib.LogOperation;
using LuaInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace HttpListenerDemo.Helper
{
    //LuaApi注册
    public class LuaApiRegister
    {
        private Lua _luaVM = null;

        public LuaApiRegister(object luaAPIClass)
        {
            _luaVM = new Lua();//初始化Lua虚拟机
            BindClassToLua(luaAPIClass);
        }

        private void BindClassToLua(object luaAPIClass)
        {
            foreach (MethodInfo mInfo in luaAPIClass.GetType().GetMethods())
            {
                foreach (Attribute attr in Attribute.GetCustomAttributes(mInfo, false))
                {
                    if (!attr.ToString().StartsWith("System.") && !attr.ToString().StartsWith("__"))
                    {
                        _luaVM.RegisterFunction((attr as LuaFunction).getFuncName(), luaAPIClass, mInfo);
                    }
                }
            }
        }

        public void ExecuteFile(string luaFileName)
        {
            Logger.Info("开始执行脚本：" + luaFileName);
            _luaVM.DoFile(luaFileName);
        }

        public void ExecuteString(string luaCommand)
        {
            try
            {
                _luaVM.DoString(luaCommand);
            }
            catch (Exception e)
            {
                Console.WriteLine("执行lua脚本指令：" + e.ToString());
            }
        }
    }

    //Lua函数描述特性类  
    public class LuaFunction : Attribute
    {
        private String FunctionName;

        public LuaFunction(String strFuncName)
        {
            FunctionName = strFuncName;
        }

        public String getFuncName()
        {
            return FunctionName;
        }
    }
}
