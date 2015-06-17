using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ExtNetBase.Mvc
{
    public class ComponentHelper
    {
        public static Model GetModel(Dictionary<string,ModelFieldType> fields)
        {
            Ext.Net.Model model = new Model();
            model.Fields.AddRange(fields.Select(x => new ModelField(x.Key, x.Value)));
            return model;
        }
    }
}
