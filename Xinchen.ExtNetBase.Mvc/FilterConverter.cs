namespace Xinchen.ExtNetBase.Mvc
{
    using Ext.Net;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using Xinchen.DbUtils;

    public class FilterConverter
    {
        /// <summary>
        /// 如果给定的FilterCondition是列表类型，则取出这个列表
        /// </summary>
        /// <param name="fc"></param>
        /// <returns></returns>
        public static List<int> GetListValues(FilterCondition fc)
        {
            JArray source = fc.Value<JArray>();
            List<int> values = new List<int>();
            source.ToList<JToken>().ForEach(delegate(JToken x)
            {
                values.Add(x.Value<int>());
            });
            return values;
        }

        /// <summary>
        /// 将给定的Ext.Net过滤器转换为可供转换为Sql语句的过滤器
        /// </summary>
        /// <param name="filterCondition"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        static SqlFilter ConvertToFilter(FilterCondition filterCondition, string field)
        {
            object valueObject = filterCondition.Value<object>();
            SqlFilter filter = new SqlFilter();
            var comparison = filterCondition.Comparison;
            filter.Name = field;
            if (valueObject is JValue)
            {
                JValue jValue = valueObject as JValue;
                object jValueObject = jValue.Value;
                switch (jValue.Type)
                {
                    case JTokenType.Integer:
                        jValueObject = Convert.ToInt32(jValueObject);
                        break;

                    case JTokenType.Float:
                        jValueObject = Convert.ToDecimal(jValueObject);
                        break;

                    case JTokenType.String:
                        jValueObject = Convert.ToString(jValueObject);
                        break;

                    case JTokenType.Boolean:
                        jValueObject = Convert.ToBoolean(jValueObject);
                        break;

                    case JTokenType.Date:
                        jValueObject = Convert.ToDateTime(jValueObject);
                        break;

                    default:
                        throw new NotSupportedException("未支持的JTokenType" + jValue.Type);
                }
                filter.Value = jValueObject;
                switch (comparison)
                {
                    case Comparison.Eq:
                        filter.Operation = Operation.Equal;
                        break;

                    case Comparison.Gt:
                        filter.Operation = Operation.GreaterThan;
                        break;

                    case Comparison.Lt:
                        filter.Operation = Operation.LessThan;
                        break;
                }
                if (jValueObject is string)
                {
                    filter.Operation = Operation.Like;
                }
            }
            else if (valueObject is JArray)
            {
                filter.Operation = Operation.List;
                JArray source = valueObject as JArray;
                List<int> values = new List<int>();
                source.ToList<JToken>().ForEach(delegate(JToken x)
                {
                    values.Add(x.Value<int>());
                });
                filter.Value = values;
            }
            return filter;
        }

        /// <summary>
        /// 将Ext.Net的参数中的过滤参数转换成条件链表
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static IList<SqlFilter> ConvertToFilters(FilterConditions e)
        {
            IList<SqlFilter> filters = new List<SqlFilter>();
            if(e==null || e.Conditions==null)
            {
                return filters;
            }
            foreach (var item in e.Conditions)
            {
                SqlFilter filter = ConvertToFilter(item, item.Field);
                filters.Add(filter);
            }
            return filters;
        }

        /// <summary>
        /// 注意只会转换第一个排序规则
        /// </summary>
        /// <param name="sorters"></param>
        /// <returns></returns>
        public static Sort ConvertToSorter(DataSorter[] sorters, Func<string, string> fieldMap = null)
        {
            foreach (var item in sorters)
            {
                return new Sort()
                {
                    Field = fieldMap == null ? item.Property : fieldMap(item.Property),
                    SortOrder = item.Direction == SortDirection.ASC ? SortOrder.ASC : SortOrder.DESCENDING
                };
            }
            return new Sort()
            {
                Field = fieldMap == null ? "Id" : fieldMap("Id"),
                SortOrder = SortOrder.DESCENDING
            };
        }
    }
}

