using Ext.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.ExtNetBase.Mvc
{
    public abstract class ForeignFilterBase
    {
        public virtual Ext.Net.Model GetModel()
        {
            var model = new Ext.Net.Model();
            model.Fields.Add(new ModelField()
            {
                Type = ModelFieldType.String,
                Name = "Name"
            });
            model.Fields.Add(new ModelField()
            {
                Name = "Id",
                Type = ModelFieldType.Int
            });
            return model;
        }

        public abstract object GetData();

        public virtual string IdField
        {
            get
            {
                return "Id";
            }
        }

        public virtual string LabelField
        {
            get
            {
                return "Name";
            }
        }
    }
}
