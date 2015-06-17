namespace Xinchen.ApplicationBase.Mvc.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.ApplicationBase.Mvc.UI.User;
    using Xinchen.ExtNetBase;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.Utils.DataAnnotations;

    public class UserManagerModel
    {
        [DataGridColumn(DisplayName = "编号")]
        [Xinchen.ExtNetBase.Mvc.Filter]
        public int Id { get; set; }

        [DataGridColumn(DisplayName = "用户名")]
        [Xinchen.ExtNetBase.Mvc.Filter]
        public string Username { get; set; }

        [DataGridColumn(DisplayName = "创建时间")]
        [Xinchen.ExtNetBase.Mvc.Filter]
        public DateTime CreateTime { get; set; }

        [DataGridColumn(DisplayName = "描述")]
        [Xinchen.ExtNetBase.Mvc.Filter]
        public string Description { get; set; }

        [DataGridColumn(DisplayName = "拥有角色")]
        [Xinchen.ExtNetBase.Mvc.Filter(FilterType = typeof(RoleFilter))]
        public string Roles { get; set; }

        [DataGridColumn(DisplayName = "状态", ValueType = typeof(BaseStatuses))]
        [Xinchen.ExtNetBase.Mvc.Filter(FilterType = typeof(BaseStatuses))]
        public BaseStatuses Status { get; set; }
    }
}

