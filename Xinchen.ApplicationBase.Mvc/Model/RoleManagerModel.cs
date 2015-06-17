using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xinchen.ApplicationBase.Mvc.Views.Role;
using Xinchen.ExtNetBase;
using Xinchen.ExtNetBase.Mvc;
using Xinchen.PrivilegeManagement.Enums;
using Xinchen.Utils.DataAnnotations;

namespace Xinchen.ApplicationBase.Mvc.Model
{
    public class RoleManagerModel
    {
        [DataGridColumn("编号")]
        [Filter]
        public virtual int Id { get; set; }

        [DataGridColumn("名称")]
        [Filter]
        public virtual string Name { get; set; }
        [DataGridColumn("描述", Width = 200)]
        [Filter]
        public virtual string Description { get; set; }

        [DataGridColumn("拥有权限",Width=200)]
        [Filter(FilterType=typeof(PrivilegeFilter))]
        public virtual string Privileges { get; set; }

        [DataGridColumn("状态", ValueType = typeof(BaseStatuses))]
        [Filter(FilterType = typeof(BaseStatuses))]
        public virtual int Status { get; set; }
    }
}
