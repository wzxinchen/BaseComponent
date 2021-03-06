﻿using PPD.XLinq.Provider.SqlServer2008R2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xinchen.DynamicObject;
using Xinchen.Utils;

namespace PPD.XLinq.Provider
{
    public abstract class ProviderBase
    {
        DatabaseTypes _databaseType;
        private Type _type;

        public DatabaseTypes DatabaseType
        {
            get { return _databaseType; }
        }
        public ProviderBase()
        {
            _databaseType = ConfigManager.DataBaseType;
        }
        public ParserBase CreateParser()
        {
            return new Parser.Parser();
        }

        internal virtual SqlExecutorBase CreateSqlExecutor()
        {
            return new SqlExecutorBase();
        }

        public abstract IEntityOperator CreateEntityOperator();
        public BuilderFactory CreateSqlBuilderFactory()
        {
            if (!string.IsNullOrWhiteSpace(ConfigManager.SqlBuilder))
            {
                if (_type == null)
                {
                    var assInfos = ConfigManager.SqlBuilder.Split(',');
                    _type = Assembly.Load(assInfos[1]).GetTypes().FirstOrDefault(x => x.FullName == assInfos[0]);
                }
                return (BuilderFactory)ExpressionReflector.CreateInstance(_type);
            }
            return new BuilderFactory(DatabaseType);
        }
    }
}
