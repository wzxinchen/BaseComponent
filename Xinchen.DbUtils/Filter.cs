using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.DbUtils
{
    public class SqlFilter
    {

        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }
        public string ParameterName { get; set; }

        /// <summary>
        /// 搜索操作，大于小于等于
        /// </summary>
        public Xinchen.DbUtils.Operation Operation { get; set; }

        /// <summary>
        /// 搜索参数值
        /// </summary>
        public object Value { get; set; }

        public override string ToString()
        {
            ParameterName = Name.Replace(".","") + Guid.NewGuid().ToString().Replace("-", "");
            if (Value == null)
            {
                return string.Format("{0} is null", Name);
            }
            if (Value is IList<int> || Value is IList<short> || Value is IList<long> || Value is IList<decimal> || Value is IList<double> || Value is IList<float>)
            {
                return string.Format("{0} in ({1})", Name, string.Join(",", Value as IList<int>));
            }
            else
            {
                string op = string.Empty;
                switch (Operation)
                {
                    case DbUtils.Operation.Equal:
                        op = "=";
                        break;
                    case DbUtils.Operation.GreaterThan:
                        op = ">";
                        break;
                    case DbUtils.Operation.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    case DbUtils.Operation.LessThan:
                        op = "<";
                        break;
                    case DbUtils.Operation.LessThanOrEqual:
                        op = "<=";
                        break;
                    case DbUtils.Operation.Like:
                        op = "like";
                        break;
                    case DbUtils.Operation.NotEqual:
                        op = "<>";
                        break;
                }
                if (op == "like")
                {
                    return string.Format("{0} {1} '%'+@{2}+'%'", Name, op, ParameterName);
                }
                return string.Format("{0} {1} @{2}", Name, op, ParameterName);
            }
        }
    }

    public class Sort
    {
        public string Field { get; set; }
        public SortOrder SortOrder { get; set; }

        /// <summary>
        /// 将排序规则转换为sql语句
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0} {1}", Field, SortOrder.ToString());
        }
    }

    public enum SortOrder
    {
        DESC, ASC
    }

    public class FilterLinked
    {
        FilterWrapper _filter;
        public FilterLinked(SqlFilter filter)
        {
            _filter = new FilterWrapper();
            _filter.Filter = filter;
        }

        public FilterLinked(FilterLinked filterLinked)
        {
            _filter = new FilterWrapper();
            _filter.FilterLinked = filterLinked;

        }

        /// <summary>
        /// 添加一个过滤器到链表
        /// </summary>
        /// <param name="logic">与上一个过滤器的逻辑关系</param>
        /// <param name="filter">要添加的过滤器</param>
        /// <returns></returns>
        public FilterLinked AddFilter(LogicOperation logic, SqlFilter filter)
        {
            _filter.Next = new FilterWrapper();
            _filter.Next.Operation = logic;
            _filter.Next.Filter = filter;
            _filter.Next.Prev = _filter;
            _filter = _filter.Next;
            return this;
        }

        /// <summary>
        /// 添加一个过滤器链表到链表
        /// </summary>
        /// <param name="logic">与上一个过滤器的逻辑关系</param>
        /// <param name="filter">要添加的过滤器链表</param>
        /// <returns></returns>
        public FilterLinked AddFilterLinked(LogicOperation logic, FilterLinked filterLinked)
        {
            _filter.Next = new FilterWrapper();
            _filter.Next.Operation = logic;
            _filter.Next.FilterLinked = filterLinked;
            _filter.Next.Prev = _filter;
            _filter = _filter.Next;
            return this;
        }

        /// <summary>
        /// 将条件链表转换为sql中的where子句，返回转换结果和参数化查询所需要的参数字典
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string ToString(Dictionary<string, object> parameters)
        {
            FilterWrapper wrapper = _filter;
            List<string> conditions = new List<string>();
            string condition = string.Empty;
            Dictionary<string, object> tempParameters = new Dictionary<string, object>();
            while (true)
            {
                bool hasChild = false;
                if (wrapper.FilterLinked != null)
                {
                    hasChild = true;
                    conditions.Add("(" + wrapper.FilterLinked.ToString(parameters) + ")");
                }
                else
                {
                    conditions.Add(wrapper.Filter.ToString());
                    conditions.Add(wrapper.Operation.ToString());
                    var value = wrapper.Filter.Value;
                    if (value != null && !(value is IList<int> || value is IList<short> || value is IList<long> || value is IList<decimal> || value is IList<double> || value is IList<float>))
                    {
                        tempParameters.Add(wrapper.Filter.ParameterName, wrapper.Filter.Value);
                    }
                }
                wrapper = wrapper.Prev;
                if (wrapper == null)
                {
                    if (!hasChild)
                        conditions.RemoveAt(conditions.Count - 1);
                    break;
                }
            }
            conditions.Reverse();
            var reverseParams = tempParameters.Reverse();
            foreach (var item in reverseParams)
            {
                parameters.Add(item.Key, item.Value);
            }
            return string.Join(" ", conditions);
        }
    }

    public class FilterWrapper
    {
        public FilterWrapper Prev { get; set; }
        public FilterWrapper Next { get; set; }
        public LogicOperation Operation { get; set; }
        public SqlFilter Filter { get; set; }
        public FilterLinked FilterLinked { get; set; }
    }

    public enum LogicOperation
    {
        And,
        Or
    }
}
