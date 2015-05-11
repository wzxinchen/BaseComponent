using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xinchen.DbEntity
{
    public class SqlBuilder
    {
        private string _table;
        private string _fields;
        private string _where;
        private string _order;
        private string _group;
        public SqlBuilder(string table, string fields)
        {
            if (string.IsNullOrEmpty(table))
            {
                throw new ArgumentNullException("table");
            }

            if (string.IsNullOrEmpty(fields))
            {
                throw new ArgumentNullException("fields");
            }
            Table = table;
            Fields = fields;
        }

        public string Where
        {
            get { return _where; }
            set { _where = value; }
        }

        public string Order
        {
            get { return _order; }
            set { _order = value; }
        }

        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public string Fields
        {
            get { return _fields; }
            set { _fields = value; }
        }

        public string Table
        {
            get { return _table; }
            set { _table = "[" + value + "]"; }
        }


        public string BuildSql()
        {
            if (string.IsNullOrEmpty(Table))
            {
                throw new ArgumentNullException("table");
            }

            if (string.IsNullOrEmpty(Fields))
            {
                throw new ArgumentNullException("fields");
            }
            StringBuilder sqlBuilder = new StringBuilder(1024);
            sqlBuilder.Append("select ");
            sqlBuilder.Append(Fields);
            sqlBuilder.Append(" from ");
            sqlBuilder.Append(Table);

            if (!string.IsNullOrEmpty(Where))
            {
                sqlBuilder.Append(" where ");
                sqlBuilder.Append(Where);
            }

            if (!string.IsNullOrEmpty(Order))
            {
                sqlBuilder.Append(" order by ");
                sqlBuilder.Append(Order);
            }

            if (!string.IsNullOrEmpty(Group))
            {
                sqlBuilder.Append(" group by ");
                sqlBuilder.Append(Group);
            }

            return sqlBuilder.ToString();
        }
    }
}
