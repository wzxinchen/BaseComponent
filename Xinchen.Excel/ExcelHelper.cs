using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Xinchen.Utils;
using Xinchen.Utils.DataAnnotations;
using Xinchen.Utils.Entity;
using Xinchen.DynamicObject;

namespace Xinchen.Excel
{
    public class ExcelHelper
    {
        public static List<T> Import<T>(Stream stream, Func<string, string, string> onImportItemPropertyData,Action<T> onImportItemData)
            where T : class,new()
        {
            HSSFWorkbook book = new HSSFWorkbook(stream);
            Type type = typeof(T);
            var piDict = ExpressionReflector.GetProperties(type);
            var attrsDict = new Dictionary<string, string>();
            var properties = new Dictionary<int, string>();
            foreach (var propertyInfo in piDict.Values)
            {
                var attr = AttributeHelper.GetAttribute<DataGridColumnAttribute>(propertyInfo);
                if (attr == null) continue;
                attrsDict.Add(attr.DisplayName, propertyInfo.Name);
            }
            Dictionary<int, Action<object, object>> settersMapped = new Dictionary<int, Action<object, object>>();
            var setters = ExpressionReflector.GetSetters(type);
            ISheet sheet = book.GetSheetAt(0);
            IRow headerRow = sheet.GetRow(0);
            for (int i = 0; i < headerRow.Cells.Count; i++)
            {
                ICell cell = headerRow.Cells[i];
                string name = cell.StringCellValue.ToLower();
                if (attrsDict.ContainsKey(name))
                {
                    name = attrsDict[name].ToLower();
                }
                foreach (var key in setters.Keys.Where(key => key.ToLower() == name))
                {
                    settersMapped.Add(i, setters[key]);
                    properties.Add(i, name);
                    break;
                }
            }
            List<T> list = new List<T>();
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                T t = new T();
                if (onImportItemData != null)
                {
                    onImportItemData(t);
                }
                var cells = sheet.GetRow(i).Cells;
                for (int j = 0; j < cells.Count; j++)
                {
                    string value = cells[j].ToString();
                    if (onImportItemPropertyData != null)
                    {
                        value = onImportItemPropertyData(value, properties[j]);
                    }
                    settersMapped[j](t, value);
                }
                list.Add(t);
            }
            return list;
            //ISheet sheet= book.CreateSheet();
            //var header= sheet.CreateRow(0);
            //for (int i = 0; i < properties.Count; i++)
            //{
            //    ICell cell= header.CreateCell(i);
            //    string name = properties[i];

            //    if (attrsDict.ContainsKey(name))
            //    {
            //        name = attrsDict[properties[i]].DisplayName;
            //    }
            //    cell.SetCellValue(name);
            //}

        }
    }
}
