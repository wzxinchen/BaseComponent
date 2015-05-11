using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace UnitTestProject1
{
    public class TestConfig : IConfigurationSectionHandler
    {
        public string Test { get; set; }
        public TestConfig Config { get; set; }

        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            return null;
        }
    }
}
