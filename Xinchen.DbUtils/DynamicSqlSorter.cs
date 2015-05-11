namespace Xinchen.DbUtils
{
    using System;
    using System.Runtime.CompilerServices;

    public class DynamicSqlSorter : DynamicSqlParam
    {
        /// <summary>
        /// 将排序实例转换为字符串
        /// </summary>
        /// <returns></returns>
        public string ToSql()
        {
            return (this.Name + " " + this.Direction.ToString());
        }

        public Xinchen.DbUtils.Direction Direction { get; set; }

        public string Name { get; set; }

        public override DynamicSqlParamType Type
        {
            get
            {
                return DynamicSqlParamType.Sort;
            }
        }
    }
}

