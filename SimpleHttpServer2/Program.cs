
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Web;

namespace ConsoleApplication1
{
    class Program
    {
        static HttpListener sSocket = null;
        public static string modulePath;

        static void Main(string[] args)
        {
            sSocket = new HttpListener();
            sSocket.Prefixes.Add("http://127.0.0.1:80/");
            sSocket.Start();
            sSocket.BeginGetContext(new AsyncCallback(GetContextCallBack), sSocket);

            modulePath = System.AppDomain.CurrentDomain.BaseDirectory;
            Console.WriteLine("modulePath : "+modulePath+"\n");
            Console.Read();
        }

        static void GetContextCallBack(IAsyncResult ar)
        {
            try
            {
                sSocket = ar.AsyncState as HttpListener;
                HttpListenerContext context = sSocket.EndGetContext(ar);
                sSocket.BeginGetContext(new AsyncCallback(GetContextCallBack), sSocket);
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;
                Console.WriteLine(request.Url.PathAndQuery);

                string username = HttpUtility.ParseQueryString(request.Url.Query).Get("username");
                string pwd = HttpUtility.ParseQueryString(request.Url.Query).Get("pwd");

                string absPath = request.Url.AbsolutePath.Substring(1);
                Console.WriteLine("absPath:"+absPath);

                if (absPath == "favicon.ico")
                {
                    string filename=Path.Combine(modulePath,absPath);
                    if(File.Exists(filename))
                    {
                        try
                        {
                            Console.WriteLine("Process ico");
                            Stream input = new FileStream(filename, FileMode.Open);
                            string mime = "image/x-icon";
                            response.ContentType = mime;
                            response.ContentLength64 = input.Length;
                            response.AddHeader("Date", DateTime.Now.ToString("r"));
                            response.AddHeader("Last-Modified", File.GetLastWriteTime(filename).ToString("r"));

                            byte[] obuffer = new byte[1024 * 32];
                            int nbytes;
                            while ((nbytes = input.Read(obuffer, 0, obuffer.Length)) > 0)
                            {
                                response.OutputStream.Write(obuffer, 0, nbytes);
                            }
                            
                            input.Close();
                            response.OutputStream.Flush();
                            response.StatusCode = (int)HttpStatusCode.OK;
                            obuffer = null;
                            
                        }
                        catch(Exception ex)
                        {
                            response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            Console.WriteLine("Exception : "+ex.Message);
                        }
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                    }
                    response.OutputStream.Close();
                    return;
                }

                Console.WriteLine("username:"+username+"\npwd:"+pwd);

                string responseString = "Hello world!";
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();

                //其它处理code  
            }
            catch { }
        }
    }
}
