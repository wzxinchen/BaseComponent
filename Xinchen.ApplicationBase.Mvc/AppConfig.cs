using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.ApplicationBase.Mvc
{
    public static class AppConfig
    {
        public static string WebTitle
        {
            get
            {
                return ConfigurationManager.AppSettings["WebTitle"];
            }
        }

        public static string WebName
        {
            get
            {
                return ConfigurationManager.AppSettings["WebName"];
            }
        }
    }
}
