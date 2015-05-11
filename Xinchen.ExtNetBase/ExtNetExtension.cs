namespace Xinchen.ExtNetBase
{
    using Ext.Net;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public static class ExtNetExtension
    {

        public static string Serialize(this DataSorter[] sorters)
        {
            List<string> values = new List<string>();
            foreach (DataSorter sorter in sorters)
            {
                values.Add(sorter.Property + " " + sorter.Direction);
            }
            return string.Join(",", values);
        }

        public static void Set(this LoadMask eventMask, string msg)
        {
            eventMask.ShowMask = true;
            eventMask.Msg = msg;
        }
    }
}

