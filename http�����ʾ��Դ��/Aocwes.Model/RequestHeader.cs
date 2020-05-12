using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aocwes.Model
{
    public class RequestHeader
    {
        public string Method { get; private set; }
        public string Url { get; private set; }
        public string Protocol { get; private set; }
        private Dictionary<string, string> properties = null;
        public Dictionary<string, string> Properties
        {
            get
            {
                if (properties == null)
                    properties = new Dictionary<string, string>();
                return properties;
            }
        }

        public void SetHeader(string method, string url, string protocol)
        {
            this.Method = method;
            this.Url = url;
            this.Protocol = protocol;
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Method:{0}<br/>", Method);
            sb.AppendFormat("Url:{0}<br/>", Url);
            sb.AppendFormat("Protocol:{0}<br/>", Protocol);
            foreach (var item in Properties)
            {
                sb.AppendFormat("{0}:{1}<br/>", item.Key, item.Value);
            }
            return sb.ToString();
        }
    }
}
