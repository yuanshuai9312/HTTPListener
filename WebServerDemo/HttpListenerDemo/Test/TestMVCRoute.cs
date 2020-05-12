using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HttpListenerDemo.Test
{
    public class TestMVCRoute
    {
        public List<string> RegisterRoutes()
        {
            return new List<string>() { "{controller}/{action}" };
        }
    }
}
