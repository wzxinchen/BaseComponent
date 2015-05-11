namespace Xinchen.DbUtils
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    /// <summary>
    /// 动态搜索参数
    /// </summary>
    public class DynamicSqlFilter : DynamicSqlParam
    {
        public DynamicSqlFilter()
        {
            this.Ors = new List<DynamicSqlFilter>();
        }

        /// <summary>
        /// 将当前搜索实例转换成sql语句并返回，将可能含有的需要参数化查询的数据存入提供的字典中，Key为参数名，Value为参数值
        /// </summary>
        /// <param name="values">用于存放参数化查询的字典</param>
        /// <param name="randParamName">参数名是否随机</param>
        /// <returns></returns>
        public string ToSql(Dictionary<string, object> values)
        {
            string str = string.Empty;
            string name = this.Name;
            object obj2 = this.Value;
            if (this.Ors.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (DynamicSqlFilter filter in this.Ors)
                {
                    list.Add(filter.ToSql(values));
                }
                return ("(" + string.Join(" or ", list) + ")");
            }
            string paramName = this.Name.Replace(".", "");
            paramName = (paramName + Guid.NewGuid().ToString().Replace("-", "")).Replace(".", "");
            switch (this.Operation)
            {
                case Xinchen.DbUtils.Operation.GreaterThan:
                    str = string.Format("{0} > @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.LessThan:
                    str = string.Format("{0} < @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.GreaterThanOrEqual:
                    str = string.Format("{0} >= @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.LessThanOrEqual:
                    str = string.Format("{0} <= @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.NotEqual:
                    str = (obj2 == null) ? string.Format("not {0} is null", name) : string.Format("{0} != @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.Equal:
                    str = (obj2 == null) ? string.Format("{0} is null", name) : string.Format("{0} = @{1}", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.Like:
                    str = string.Format("{0} like '%'+@{1}+'%'", name, paramName);
                    break;

                case Xinchen.DbUtils.Operation.List:
                    str = name + " in (" + string.Join<int>(",", (List<int>)obj2) + ")";
                    break;
            }
            if (this.Operation != Xinchen.DbUtils.Operation.List)
            {
                values.Add(paramName, obj2);
            }
            return str;
        }

        /// <summary>
        /// 字段名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 搜索操作，大于小于等于
        /// </summary>
        public Xinchen.DbUtils.Operation Operation { get; set; }

        /// <summary>
        /// 需要用Or连接的子参数，如果含有子参数，则只处理子参数
        /// </summary>
        public List<DynamicSqlFilter> Ors { get; private set; }

        /// <summary>
        /// 返回动态sql查询类型
        /// </summary>
        public override DynamicSqlParamType Type
        {
            get
            {
                return DynamicSqlParamType.Filter;
            }
        }

        /// <summary>
        /// 搜索参数值
        /// </summary>
        public object Value { get; set; }
    }
}

