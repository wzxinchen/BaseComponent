using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Xinchen.Utils;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.Excel
{
    /// <summary>
    /// 用于导入Excel文件
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class Importer<TResult>
        where TResult : new()
    {
        Stream _stream;
        /// <summary>
        /// 相关消息，一般是错误消息
        /// </summary>
        public string Message { get; private set; }
        /// <summary>
        /// 导入开始时触发，参数是总条数
        /// </summary>
        public event Action<int> OnStart;
        /// <summary>
        /// 导入结束时触发
        /// </summary>
        public event Action OnEnd;
        /// <summary>
        /// 导入单条记录完成时触发
        /// </summary>
        public event Action<TResult> OnImportItemData;
        PropertyInfo[] _propertyInfos;
        Dictionary<PropertyInfo, string> _propertyInfoMap = new Dictionary<PropertyInfo, string>();
        List<string> _columns = new List<string>();
        public Importer(Stream stream)
        {
            _stream = stream;
            _propertyInfos = typeof(TResult).GetProperties();
            var dgType = typeof(DataGridColumnAttribute);
            foreach (var item in _propertyInfos)
            {
                var attr = (DataGridColumnAttribute)item.GetCustomAttributes(dgType, true).FirstOrDefault();
                if (attr != null)
                {
                    _propertyInfoMap.Add(item, attr.DisplayName);
                    _columns.Add(attr.DisplayName);
                }
                else
                {
                    _propertyInfoMap.Add(item, item.Name);
                    _columns.Add(item.Name);
                }
            }
        }

        /// <summary>
        /// 开始执行导入，并返回导入结果
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            HSSFWorkbook book = new HSSFWorkbook(_stream);
            var sheet = book.GetSheetAt(0);
            var row = sheet.GetRow(0);
            Dictionary<string, int> columnMap = new Dictionary<string, int>();
            var notHaveColumns = _columns.Where(x =>
            {
                var cell = row.Cells.FirstOrDefault(y => y.StringCellValue == x);
                if (cell == null) return true;
                columnMap.Add(x, row.Cells.IndexOf(cell));
                return false;
            }).ToList();
            if (notHaveColumns.Any())
            {
                Message = "导入的文件中缺少以下列：" + string.Join(",", notHaveColumns);
                return false;
            }
            else
            {
                if (OnStart != null)
                {
                    OnStart(sheet.PhysicalNumberOfRows - 1);
                }
                List<string> msgs = new List<string>();
                for (int i = 1; i < sheet.PhysicalNumberOfRows; i++)
                {
                    row = sheet.GetRow(i);
                    TResult result = new TResult();
                    foreach (var propertyInfo in _propertyInfoMap.Keys)
                    {
                        string value = null;
                        object v;
                        try
                        {
                            var propertyInfoMapIndex = _propertyInfoMap[propertyInfo];
                            var columnMapIndex = columnMap[propertyInfoMapIndex];
                            value = Convert.ToString(row.GetCell(columnMapIndex));
                            Type type = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                            if (type == null)
                            {
                                type = propertyInfo.PropertyType;
                            }
                            v = Convert.ChangeType(value, type);
                            propertyInfo.SetValue(result, v, null);
                        }
                        catch (FormatException)
                        {
                            msgs.Add("第" + (i + 1) + "行的" + _propertyInfoMap[propertyInfo] + "列导入失败");
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            break;
                        }
                    }
                    if (OnImportItemData != null)
                    {
                        OnImportItemData(result);
                    }
                    else
                    {
                        throw new ArgumentNullException("请注册OnImportItemData事件以接收数据");
                    }
                }
                if (msgs.Count > 0)
                {
                    Message = string.Join(Environment.NewLine, msgs.Take(10));
                    return false;
                }
                if (OnEnd != null)
                {
                    OnEnd();
                }
                return true;
            }
        }
    }
}
