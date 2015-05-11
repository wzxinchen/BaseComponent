namespace Xinchen.DbUtils
{
    using System;

    /// <summary>
    /// 动态查询基类，字段名会被随机生成，如果是确定要查询的条件，请不要使用该类及其子类
    /// </summary>
    public abstract class DynamicSqlParam
    {
        protected DynamicSqlParam()
        {
        }

        ///// <summary>
        ///// 创建一个动态搜索参数实例
        ///// </summary>
        ///// <returns></returns>
        //public static DynamicSqlFilter CreateFilter()
        //{
        //    return new DynamicSqlFilter();
        //}

        /// <summary>
        /// 创建一个动态排序参数实例
        /// </summary>
        /// <returns></returns>
        public static DynamicSqlSorter CreateSorter()
        {
            return new DynamicSqlSorter();
        }

        /// <summary>
        /// 返回是排序还是搜索
        /// </summary>
        public abstract DynamicSqlParamType Type { get; }
    }
}

