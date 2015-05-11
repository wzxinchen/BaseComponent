using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Xinchen.DbUtils
{
    public interface IRepository : IDisposable
    {
        IUnit<T> Use<T>()
        where T : class,new();
        int SaveChanges();

        IList<T> Queries<T>(string sql);
    }
}
