namespace Xinchen.ExtNetBase
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

        private static string MapField(IDynamicParamConverter converter, string field)
        {
            if ((converter != null) && ((converter.FieldMap != null) && converter.FieldMap.ContainsKey(field)))
            {
                return converter.FieldMap[field];
            }
            return field;
        }

        public static DynamicSqlParam[] ToSqlParams(DataSorter[] sorters, Dictionary<string, string> fieldMap)
        {
            List<DynamicSqlParam> list = new List<DynamicSqlParam>();
            foreach (DataSorter sorter in sorters)
            {
                DynamicSqlSorter item = new DynamicSqlSorter
                {
                    Direction = (sorter.Direction == SortDirection.ASC) ? Xinchen.DbUtils.Direction.ASC : Xinchen.DbUtils.Direction.DESC
                };
                if ((fieldMap != null) && fieldMap.ContainsKey(sorter.Property))
                {
                    item.Name = fieldMap[sorter.Property];
                }
                else
                {
                    item.Name = sorter.Property;
                }
                list.Add(item);
            }
            return list.ToArray();
        }

        //public static DynamicSqlParam[] ToSqlParams(FilterConditions filters, IDynamicParamConverter converter)
        //{
        //    List<DynamicSqlParam> list = new List<DynamicSqlParam>();
        //    foreach (FilterCondition condition in filters.Conditions)
        //    {
        //        Comparison comparison = condition.Comparison;
        //        string field = condition.Field;
        //        if (converter != null)
        //        {
        //            if (converter.FieldMap.ContainsKey(field))
        //            {
        //                field = converter.FieldMap[field];
        //                if (string.IsNullOrEmpty(field))
        //                {
        //                    continue;
        //                }
        //            }
        //            else if (converter.Converters.ContainsKey(field))
        //            {
        //                list.AddRange(converter.Converters[field](condition));
        //                continue;
        //            }
        //        }
        //        FilterType type = condition.Type;
        //        object obj2 = condition.Value<object>();
        //        DynamicSqlFilter item = DynamicSqlParam.CreateFilter();
        //        item.Name = field;
        //        if (obj2 is JValue)
        //        {
        //            JValue value2 = obj2 as JValue;
        //            object obj3 = value2.Value;
        //            switch (value2.Type)
        //            {
        //                case JTokenType.Integer:
        //                    obj3 = Convert.ToInt32(obj3);
        //                    break;

        //                case JTokenType.Float:
        //                    obj3 = Convert.ToDecimal(obj3);
        //                    break;

        //                case JTokenType.String:
        //                    obj3 = Convert.ToString(obj3);
        //                    break;

        //                case JTokenType.Boolean:
        //                    obj3 = Convert.ToBoolean(obj3);
        //                    break;

        //                case JTokenType.Date:
        //                    obj3 = Convert.ToDateTime(obj3);
        //                    break;

        //                default:
        //                    throw new NotSupportedException("未支持的JTokenType" + value2.Type);
        //            }
        //            item.Value = obj3;
        //            switch (comparison)
        //            {
        //                case Comparison.Eq:
        //                    item.Operation = Operation.Equal;
        //                    break;

        //                case Comparison.Gt:
        //                    item.Operation = Operation.GreaterThan;
        //                    break;

        //                case Comparison.Lt:
        //                    item.Operation = Operation.LessThan;
        //                    break;
        //            }
        //            if (obj3 is string)
        //            {
        //                item.Operation = Operation.Like;
        //            }
        //        }
        //        else if (obj2 is JArray)
        //        {
        //            item.Operation = Operation.List;
        //            JArray source = obj2 as JArray;
        //            List<int> values = new List<int>();
        //            source.ToList<JToken>().ForEach(delegate(JToken x)
        //            {
        //                values.Add(x.Value<int>());
        //            });
        //            item.Value = values;
        //        }
        //        list.Add(item);
        //    }
        //    return list.ToArray();
        //}

        //public static DynamicSqlParam[] ToSqlParams(FilterConditions filters, Func<string, string> fieldMap)
        //{
        //    List<DynamicSqlParam> list = new List<DynamicSqlParam>();
        //    foreach (FilterCondition condition in filters.Conditions)
        //    {
        //        Comparison comparison = condition.Comparison;
        //        string field = condition.Field;
        //        if (fieldMap != null)
        //        {
        //            field = fieldMap(field);
        //            if (string.IsNullOrEmpty(field))
        //            {
        //                continue;
        //            }
        //        }
        //        FilterType type = condition.Type;
        //        object obj2 = condition.Value<object>();
        //        DynamicSqlFilter item = DynamicSqlParam.CreateFilter();
        //        item.Name = field;
        //        if (obj2 is JValue)
        //        {
        //            JValue value2 = obj2 as JValue;
        //            object obj3 = value2.Value;
        //            switch (value2.Type)
        //            {
        //                case JTokenType.Integer:
        //                    obj3 = Convert.ToInt32(obj3);
        //                    break;

        //                case JTokenType.Float:
        //                    obj3 = Convert.ToDecimal(obj3);
        //                    break;

        //                case JTokenType.String:
        //                    obj3 = Convert.ToString(obj3);
        //                    break;

        //                case JTokenType.Boolean:
        //                    obj3 = Convert.ToBoolean(obj3);
        //                    break;

        //                case JTokenType.Date:
        //                    obj3 = Convert.ToDateTime(obj3);
        //                    break;

        //                default:
        //                    throw new NotSupportedException("未支持的JTokenType" + value2.Type);
        //            }
        //            item.Value = obj3;
        //            switch (comparison)
        //            {
        //                case Comparison.Eq:
        //                    item.Operation = Operation.Equal;
        //                    break;

        //                case Comparison.Gt:
        //                    item.Operation = Operation.GreaterThan;
        //                    break;

        //                case Comparison.Lt:
        //                    item.Operation = Operation.LessThan;
        //                    break;
        //            }
        //            if (obj3 is string)
        //            {
        //                item.Operation = Operation.Like;
        //            }
        //        }
        //        else if (obj2 is JArray)
        //        {
        //            item.Operation = Operation.List;
        //            JArray source = obj2 as JArray;
        //            List<int> values = new List<int>();
        //            source.ToList<JToken>().ForEach(delegate(JToken x)
        //            {
        //                values.Add(x.Value<int>());
        //            });
        //            item.Value = values;
        //        }
        //        list.Add(item);
        //    }
        //    return list.ToArray();
        //}

        //public static List<DynamicSqlParam> ToSqlParams(StoreReadDataEventArgs e, IDynamicParamConverter converter, bool autoGenerateSort = true)
        //{
        //    List<DynamicSqlParam> list = new List<DynamicSqlParam>();
        //    if (!string.IsNullOrEmpty(e.Parameters["filter"]))
        //    {
        //        list.AddRange(ToSqlParams(new FilterConditions(e.Parameters["filter"]), converter));
        //    }
        //    List<DynamicSqlParam> collection = ToSqlParams(e.Sort, converter.FieldMap).ToList<DynamicSqlParam>();
        //    if (autoGenerateSort && (collection.Count <= 0))
        //    {
        //        DynamicSqlSorter item = DynamicSqlParam.CreateSorter();
        //        item.Direction = Xinchen.DbUtils.Direction.ASC;
        //        item.Name = MapField(converter, "Id");
        //        collection.Add(item);
        //    }
        //    list.AddRange(collection);
        //    return list;
        //}

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
        public static FilterLinked ConvertToFilterLinked(StoreReadDataEventArgs e, Func<FilterCondition, object> converter = null)
        {
            FilterLinked filterLinked = null;
            if (string.IsNullOrEmpty(e.Parameters["filter"]))
            {
                return filterLinked;
            }
            FilterConditions filters = new FilterConditions(e.Parameters["filter"]);
            foreach (var item in filters.Conditions)
            {
                SqlFilter filter = null;
                string field = item.Field;
                if (converter != null)
                {
                    //执行开发者指定的转换，返回null将进行默认的转换
                    var result = converter(item);
                    if (result != null)
                    {
                        if (result is SqlFilter)
                        {
                            filter = result as SqlFilter;
                        }
                        else if (result is string)
                        {
                            field = Convert.ToString(result);
                        }
                        else
                        {
                            throw new ArgumentException("ConvertToFilterLinked方法中的converter参数中的返回值只能是string型或FilterLinked型，为string型表示只自定义转换字段名，为SqlFilter型时表示完全自定义");
                        }
                    }
                    else
                    {
                        filter = ConvertToFilter(item, field);
                    }
                }

                if (filter == null)
                {
                    filter = ConvertToFilter(item, field);
                }
                if (filterLinked == null)
                {
                    filterLinked = new FilterLinked(filter);
                }
                else
                {
                    filterLinked.AddFilter(LogicOperation.And, filter);
                }
            }
            return filterLinked;
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

