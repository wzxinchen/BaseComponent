using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Xinchen.Utils
{
    public class ConfigHelper
    {
        public static string GetConfigAppSettingValue(string name)
        {
            return ConfigurationManager.AppSettings[name];
        }

        public static void SaveConfigAppSetting(string name,string value)
        {
            Configuration config= ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = config.AppSettings.Settings;
            if(settings.AllKeys.Contains(name))
            {
                settings[name].Value = value;
            }
            else
            {
                settings.Add(name, value);
            }
            config.Save();
        }
    }
}
