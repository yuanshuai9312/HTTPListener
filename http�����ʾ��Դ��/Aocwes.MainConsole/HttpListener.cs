using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Aocwes.Model;

namespace Aocwes.MainConsole
{
    public class HttpListener
    {
        public HttpListener()
        {
            IPAddress ipaddress = IPAddress.Parse("10.224.57.2");
            IPEndPoint ipep = new IPEndPoint(ipaddress, 8080);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(ipep);
            serverSocket.Listen(10);
            while (true)
            {
                try
                {
                    var clientSocket = serverSocket.Accept();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveData), clientSocket);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("listening Error: " + ex.Message);
                }
            }
        }

        private void ReceiveData(object socket)
        {
            RequestHeader requestHeader = new RequestHeader();
            var clientSocket = socket as Socket;
            Byte[] buffer = new Byte[1024];
            IPEndPoint clientep = (IPEndPoint)clientSocket.RemoteEndPoint;
            string clientMessage = string.Empty;
            try
            {
                clientSocket.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                clientMessage = System.Text.Encoding.Default.GetString(buffer);
                Console.WriteLine(clientMessage);
            }
            catch (Exception ex)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("<html><head><title>Server throw an exception:{1}</title></head><body>{0}</body></html>", ex.Message, ex.GetType().FullName);
                var data = Encoding.ASCII.GetBytes(sb.ToString());
                clientSocket.Send(data, data.Length, SocketFlags.None);
            }

            using (StringReader sr = new StringReader(clientMessage))
            {
                StringBuilder sb = new StringBuilder();
                var line = sr.ReadLine();
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] headerInfo = line.Split(new char[] { ' ' });
                    if (headerInfo.Length == 3)
                    {
                        requestHeader.SetHeader(headerInfo[0], headerInfo[1], headerInfo[2]);
                        switch (requestHeader.Method.ToUpper())
                        {
                            case "GET":
                                for (line = sr.ReadLine(); !string.IsNullOrWhiteSpace(line); line = sr.ReadLine())
                                {
                                    string[] tokens = line.Split(new string[] { ": " }, StringSplitOptions.None);
                                    if (tokens.Length == 2)
                                    {
                                        requestHeader.Properties.Add(tokens[0], tokens[1]);
                                    }
                                    else
                                    {
                                        Console.WriteLine(line);
                                    }
                                }
                                writesuccess(sb, requestHeader);
                                sb.AppendFormat("<html><head><title>You request {1}</title></head><body><h2>Welcome to aocwes http server</h2>{0}<h3><div><h3>Your requested content:</h3>{2}</div>Author: <a href='mailto:flyear.cheng2gmail.com'>Flyear</a></h3></body></html>", requestHeader.ToString(), requestHeader.Url, clientMessage);
                                var data = Encoding.ASCII.GetBytes(sb.ToString());
                                clientSocket.Send(data, data.Length, SocketFlags.None);
                                Console.WriteLine(string.Format("ClientAddress:{0}, Request Url:{1}", clientep.Address, requestHeader.Url));
                                break;
                            case "POST":
                            case "HEAD":
                            case "PUT":
                            case "DELETE":
                            case "TRACE":
                            case "CONNECT":
                            case "OPTIONS":
                            default:
                                writesuccess(sb, requestHeader);
                                sb.AppendFormat("<html><head><title>You request {1}</title></head><body><h2>Welcome to aocwes http server</h2>{0}<h3>Author: <a href='mailto:flyear.cheng2gmail.com'>Flyear</a></h3></body></html>", requestHeader.ToString(), requestHeader.Url);
                                var data1 = Encoding.ASCII.GetBytes(sb.ToString());
                                clientSocket.Send(data1, data1.Length, SocketFlags.None);

                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine(line);
                    }
                }
            }
            clientSocket.Shutdown(SocketShutdown.Both);
        }

        public void writesuccess(StringBuilder sw, RequestHeader rh)
        {
            sw.AppendFormat("{0} 200 ok\r\n", rh.Protocol);
            sw.AppendLine("connection: close");
            sw.AppendLine();
        }

        public void writefailure(StringBuilder sw)
        {
            sw.AppendLine("http/1.0 404 file not found");
            sw.AppendLine("connection: close");
            sw.AppendLine();
        }
    }
}
