using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService.HttpWebServer.Common.Util
{
    public static class WebUtil
    {
        private static log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法)
        /// </summary>
        /// <param name="url">文件上传到的服务器</param>
        /// <param name="filePath">要上传的本地文件（全路径）</param>
        /// <param name="fileName">文件上传后的名称</param>
        /// <returns>服务器反馈信息</returns>
        public static string UploadFile(string url, string filePath, string fileName)
        {
            // 要上传的文件
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            //时间戳
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + strBoundary + "\r\n");
            //请求头部信息
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(fileName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");

            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            // 根据uri创建HttpWebRequest对象
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(url));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存【重要、关键】
            httpReq.AllowWriteStreamBuffering = false;
            //设置获得响应的超时时间（300秒）
            httpReq.Timeout = 300000;

            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                //每次上传4k
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];
                //已上传的字节数
                long offset = 0;
                //开始上传时间
                DateTime startTime = DateTime.Now;
                int size = br.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    TimeSpan span = DateTime.Now - startTime;
                    //1024*1024=1048576
                    size = br.Read(buffer, 0, bufferLength);
                }

                //添加尾部的时间戳
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                using (StreamReader sr = new StreamReader(s, Encoding.UTF8))
                {
                    string message = sr.ReadLine();
                    s.Close();
                    sr.Close();
                    return message;
                }
            }
            catch (Exception e)
            {
                logger.Error("文件上传失败", e);
            }
            finally
            {
                fs.Close();
                br.Close();
            }
            return null;

        }
    }
}
