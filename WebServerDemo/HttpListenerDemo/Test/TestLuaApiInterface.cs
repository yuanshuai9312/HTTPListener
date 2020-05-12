using CommonLib.LogOperation;
using HttpListenerDemo.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpListenerDemo.Test
{
    public class TestLuaApiInterface
    {
        [LuaFunction("Test")]
        public void Test(string msg)
        {
            Logger.Info("TestLuaApiInterface：" + msg);
        }
    }
}
