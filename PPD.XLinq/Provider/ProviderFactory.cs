using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPD.XLinq.Provider
{
    public class ProviderFactory
    {

        internal static ProviderBase CreateProvider(string name)
        {
            switch(name)
            {
                case "SqlServer2008R2":
                    return new SqlServer2008R2Provider();
            }
            return null;
        }
    }
}
