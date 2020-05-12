using CommonLib.LogOperation;
using HttpListenerDemo.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpListenerDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            Logger.RegisterConsoleListener();

            var methodHttpServer = new HttpServer();
            methodHttpServer.Start(new MethodHttpServer());

            var luaHttpServer = new HttpServer();
            luaHttpServer.Start(new LuaHttpServer());

            var mvcHttpServer = new HttpServer();
            mvcHttpServer.Start(new MVCHttpServer());

            while (Console.ReadLine().ToUpper() != "EXIT")
            {

            }
        }
    }
}
