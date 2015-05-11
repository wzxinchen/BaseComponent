namespace Xinchen.ApplicationBase.Model
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.CompilerServices;
    using Xinchen.ApplicationBase.UI.User;
    using Xinchen.ExtNetBase;
    using Xinchen.PrivilegeManagement.Enums;
    using Xinchen.Utils.DataAnnotations;

    public class UserManagerModel
    {
        [DataGridColumn(DisplayName = "编号")]
        [Filter]
        public int Id { get; set; }

        [DataGridColumn(DisplayName = "用户名")]
        [Filter]
        public string Username { get; set; }

        [DataGridColumn(DisplayName = "创建时间")]
        [Filter]
        public DateTime CreateTime { get; set; }

        [DataGridColumn(DisplayName = "描述")]
        [Filter]
        public string Description { get; set; }

        [DataGridColumn(DisplayName = "拥有角色")]
        [Filter(FilterType = typeof(RoleFilter))]
        public string Roles { get; set; }

        [DataGridColumn(DisplayName = "状态", ValueType = typeof(BaseStatuses))]
        [Filter(FilterType = typeof(BaseStatuses))]
        public int Status { get; set; }
    }
}

