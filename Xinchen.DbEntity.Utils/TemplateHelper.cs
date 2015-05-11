using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Xinchen.Utils;

namespace Xinchen.DbEntity.Utils
{
    class TemplateHelper
    {
        public static string ReplaceModelParams(DataColumnCollection columns)
        {
            List<string> properties = new List<string>();
            foreach (DataColumn column in columns)
            {
                string typestring = null;
                switch (column.DataType.FullName)
                {
                    case "System.Int32":
                        typestring = "int?";
                        break;
                    case "System.DateTime":
                        typestring = "DateTime?";
                        break;
                    case "System.Boolean":
                        typestring = "bool?";
                        break;
                    case "System.Decimal":
                        typestring = "decimal?";
                        break;
                    case "System.Int64":
                        typestring = "long?";
                        break;
                    case "System.Int16":
                        typestring = "short?";
                        break;
                    case "System.String":
                        typestring = "string";
                        break;
                    default:
                        typestring = column.DataType.Name;
                        break;
                }
                properties.Add("public virtual " + typestring + " " + column.ColumnName + " {get;set;}");
            }

            return string.Join(Environment.NewLine, properties);
        }

        public static string ReplaceModelParams(string template, string ns, string modelName, List<Column> columns)
        {
            string modelTemplate = template;
            modelTemplate = modelTemplate.Replace("{namespace}", ns);
            modelTemplate = modelTemplate.Replace("{modelName}", StringHelper.ToSingular(modelName));
            modelTemplate = modelTemplate.Replace("{tableName}", StringHelper.ToPlural(modelName));
            List<string> properties = new List<string>();
            foreach (var column in columns)
            {
                List<string> attributes = new List<string>();
                string attribute = null;
                if (column.IsKey)
                {
                    attributes.Add("Key");
                }
                if (column.IsAutoIncrement)
                {
                    attributes.Add("AutoIncrement");
                }
                if (attributes.Count > 0)
                {
                    attribute = "[" + string.Join(",", attributes) + "]";
                }
                properties.Add(
                    (string.IsNullOrEmpty(attribute) ? "" : ("[" + string.Join(",", attributes) + "]" + Environment.NewLine))
                    + "public virtual " + column.GetTypeString() + " " + column.Name + " {get;set;}");
            }
            return modelTemplate.Replace("{properties}", string.Join(Environment.NewLine, properties));
        }
    }
}
