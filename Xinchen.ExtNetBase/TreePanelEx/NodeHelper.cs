using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ext.Net;
using Xinchen.ExtNetBase.TreePanelEx;
using Xinchen.DbEntity;
using Xinchen.Utils.Entity;
using System.Data;
using System.Transactions;
using Xinchen.Utils;
using System.Data.SqlClient;
using Xinchen.ObjectMapper;

namespace Xinchen.ExtNetBase
{
    public class NodeHelper : INodeHelper
    {
        string _tableName;
        DbHelper helper = DbHelper.GetInstance(null);
        EntityMapper<Xinchen.ExtNetBase.TreePanelEx.Node> _entityMapper = new EntityMapper<TreePanelEx.Node>();
        public NodeHelper(string tableName)
        {
            _tableName = tableName;
        }
        public IList<Xinchen.ExtNetBase.TreePanelEx.Node> GetNodeItems(int parentId)
        {
            string sql = string.Empty;
            if (parentId == 0)
            {
                sql = "select Id,Name,ParentId from " + _tableName + " where parentId=0 or parentId is null";
            }
            else
            {
                sql = "select Id,Name,ParentId from " + _tableName + " where parentId=" + parentId;
            }
            var ds = helper.ExecuteQuery(sql);
            return Mapper.MapList<Xinchen.ExtNetBase.TreePanelEx.Node>(ds);
        }

        public void ChangeParent(string[] sources, int target)
        {
            helper.ExecuteUpdate("update " + _tableName + " set ParentId=" + target + " where Id in (" + string.Join(",", sources.ToArray()) + ")");
        }

        public int GetChildCount(int nodeId)
        {
            return helper.ExecuteScalarCount("select count(1) from " + _tableName + " where parentId=" + nodeId);
        }

        public void RemoveNode(int nodeId)
        {
            helper.ExecuteUpdate("delete from " + _tableName + " where id=" + nodeId);
        }

        public bool Exists(int id, string name)
        {
            return helper.ExecuteScalarCount("select count(1) from where id=" + id + " and name=@name", helper.CreateParameter("@name", DbType.String, name)) > 0;
        }

        public void CreateNode(string name, int parentId)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                string tblName = _tableName.Replace("dbo.", "");
                string sql = string.Empty;
                sql = "select count(1) from " + tblName + " where name=@name";
                if (helper.ExecuteScalarCount(sql, helper.CreateParameter(DbType.String, @"@name", name)) > 0)
                {
                    throw new ApplicationException("名称重复");
                }
                sql = "select [count] from dbo.Sequences where Name='" + tblName + "'";
                DataSet ds = this.helper.ExecuteQuery(sql);
                int count = 0;
                if (Util.HasRow(ds))
                {
                    count = ConvertHelper.ToInt32(ds.Tables[0].Rows[0][0]) + 1;
                    sql = "update dbo.Sequences set [Count]=" + count.ToString() + " where Name='" + tblName + "'";
                    this.helper.ExecuteUpdate(sql);
                }
                else
                {
                    count = 1;
                    sql = "insert into dbo.[Sequences](Name,[Count]) values('" + tblName + "',1);select @@IDENTITY";
                    this.helper.ExecuteUpdate(sql);
                }
                helper.ExecuteUpdate("insert into " + tblName + "(Id,Name,ParentId) values(" + count + ",'" + name + "'," + parentId + ")");
                scope.Complete();
            }
        }

        public void UpdateNode(int id, string name)
        {
            try
            {
                helper.ExecuteUpdate("update " + _tableName + " set name=@name  where id=" + id, helper.CreateParameter(DbType.String, "@name", name));
            }
            catch (SqlException sqle)
            {
                if (sqle.Message.Contains("不能在具有唯一索引"))
                    throw new ApplicationException("名称重复");
                throw new Exception("修改失败", sqle);
            }
        }

        public IEnumerable<Ext.Net.ModelField> CustomFields
        {
            get { return new List<ModelField>(); }
        }
    }
}
